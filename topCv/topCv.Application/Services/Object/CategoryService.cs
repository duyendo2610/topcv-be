using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Application.Common;
using topCv.Application.DTOs.Obj;
using topCv.Application.Interfaces.Obj;
using topCv.Application.Mappings;
using topCv.Domain.Entities.Obj;

namespace topCv.Application.Services.Obj
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
                throw new ArgumentException("Name is required.");


            if (req.ParentId is int parentId)
            {
                var parentExists = await _db.Categories
                    .AsNoTracking()
                    .AnyAsync(x => x.Id == parentId, ct);
                if (!parentExists)
                    throw new InvalidOperationException("Parent category not found.");
            }

            var exists = await _db.Categories
                .AsNoTracking()
                .AnyAsync(x => x.Name == name && x.ParentId == req.ParentId, ct);

            if (exists)
                throw new InvalidOperationException("Category already exists in this parent.");

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
                throw new KeyNotFoundException("Category not found.");

            var name = req.Name.Trim();
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required.");

            // parent validation
            if (req.ParentId == id)
                throw new InvalidOperationException("ParentId cannot be itself.");

            if (req.ParentId is int parentId)
            {
                var parentExists = await _db.Categories
                    .AsNoTracking()
                    .AnyAsync(x => x.Id == parentId, ct);

                if (!parentExists)
                    throw new InvalidOperationException("Parent category not found.");
            }

            var exists = await _db.Categories
                .AsNoTracking()
                .AnyAsync(x => x.Id != id && x.Name == name && x.ParentId == req.ParentId, ct);

            if (exists)
                throw new InvalidOperationException("Category name already exists in this parent.");

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
                throw new KeyNotFoundException("Category not found.");

            // chặn xoá nếu có con (tùy nghiệp vụ)
            var hasChildren = await _db.Categories
                .AsNoTracking()
                .AnyAsync(x => x.ParentId == id, ct);

            if (hasChildren)
                throw new InvalidOperationException("Cannot delete category that has children.");

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
