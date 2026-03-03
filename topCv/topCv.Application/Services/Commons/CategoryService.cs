using Microsoft.EntityFrameworkCore;
using topCv.Application.Common;
using topCv.Application.DTOs.Commons;
using topCv.Application.Interfaces.Commons;
using topCv.Application.Mappings;

namespace topCv.Application.Services.Commons
{
    public sealed class CategoryService : ICategoryService
    {
        private readonly IAppDbContext _db;

        public CategoryService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<List<CategoryResponse>> GetAllAsync(CancellationToken ct)
        {
            var items = await _db.Categories
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync(ct);

            return items.Select(x => x.ToResponse()).ToList();
        }

        public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest req, CancellationToken ct)
        {
            var name = req.Name.Trim();
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tên không được để trống.");


            if (req.ParentId is int parentId)
            {
                var parentExists = await _db.Categories
                    .AsNoTracking()
                    .AnyAsync(x => x.Id == parentId, ct);
                if (!parentExists)
                    throw new InvalidOperationException("Không tìm thấy danh mục cha.");
            }

            var exists = await _db.Categories
                .AsNoTracking()
                .AnyAsync(x => x.Name == name && x.ParentId == req.ParentId, ct);

            if (exists)
                throw new InvalidOperationException("Danh mục đã tồn tại trong nhóm cha này.");

            var entity = new CreateCategoryRequest
            {
                Name = name,
                ParentId = req.ParentId
            }.ToEntity();

            _db.Categories.Add(entity);
            await _db.SaveChangesAsync(ct);

            return entity.ToResponse();
        }

        public async Task<CategoryResponse> UpdateAsync(int id, UpdateCategoryRequest req, CancellationToken ct)
        {
            var entity = await _db.Categories
                .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (entity is null)
                throw new KeyNotFoundException("Không tìm thấy danh mục.");

            var name = req.Name.Trim();
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tên không được để trống.");

            // parent validation
            if (req.ParentId == id)
                throw new InvalidOperationException("Danh mục cha không thể là chính nó.");

            if (req.ParentId is int parentId)
            {
                var parentExists = await _db.Categories
                    .AsNoTracking()
                    .AnyAsync(x => x.Id == parentId, ct);

                if (!parentExists)
                    throw new InvalidOperationException("Không tìm thấy danh mục cha.");
            }

            var exists = await _db.Categories
                .AsNoTracking()
                .AnyAsync(x => x.Id != id && x.Name == name && x.ParentId == req.ParentId, ct);

            if (exists)
                throw new InvalidOperationException("Tên danh mục đã tồn tại trong nhóm cha này.");

            new UpdateCategoryRequest
            {
                Name = name,
                ParentId = req.ParentId
            }.ApplyTo(entity);

            await _db.SaveChangesAsync(ct);

            return entity.ToResponse();
        }

        public async Task DeleteAsync(int id, CancellationToken ct)
        {
            var entity = await _db.Categories
                .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (entity is null)
                throw new KeyNotFoundException("Không tìm thấy danh mục.");

            // chặn xoá nếu có con (tùy nghiệp vụ)
            var hasChildren = await _db.Categories
                .AsNoTracking()
                .AnyAsync(x => x.ParentId == id, ct);

            if (hasChildren)
                throw new InvalidOperationException("Không thể xóa danh mục đang có danh mục con.");

            _db.Categories.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<List<CategoryTreeResponse>> GetTreeAsync(CancellationToken ct)
        {
            // Load tất cả categories (1 query)
            var rows = await _db.Categories
                .AsNoTracking()
                .Select(x => new CategoryTreeResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    ParentId = x.ParentId == 0 ? null : x.ParentId // nếu bạn dùng 0 => root
                })
                .ToListAsync(ct);

            // Tạo map id -> node
            var map = rows.ToDictionary(x => x.Id);

            // Danh sách root
            var roots = new List<CategoryTreeResponse>();

            foreach (var node in rows)
            {
                if (node.ParentId is null)
                {
                    roots.Add(node);
                    continue;
                }

                // Nếu có parent, gắn vào Children của parent
                if (map.TryGetValue(node.ParentId.Value, out var parent))
                {
                    parent.Children.Add(node);
                }
                else
                {
                    // dữ liệu lỗi (parentId trỏ tới id không tồn tại) => coi như root để không mất data
                    roots.Add(node);
                }
            }

            return roots;
        }
    }
}
