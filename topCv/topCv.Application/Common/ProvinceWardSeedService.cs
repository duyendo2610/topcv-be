using System.Net.Http.Json;
using topCv.Domain.Entities.Commons;

namespace topCv.Application.Common
{
    public class ProvinceWardSeedService
    {
        private readonly IAppDbContext _db;
        private readonly HttpClient _http;

        public ProvinceWardSeedService(IAppDbContext db, HttpClient http)
        {
            _db = db;
            _http = http;
        }

        public async Task SeedAsync()
        {
            if (_db.Provinces.Any()) return;

            var url = "https://provinces.open-api.vn/api/v2/?depth=2";
            var provinces = await _http.GetFromJsonAsync<List<ProvinceApiDto>>(url);

            if (provinces == null) return;

            foreach (var p in provinces)
            {
                var province = new Province
                {
                    Code = p.Code,
                    Name = p.Name,
                    Codename = p.Codename,
                    DivisionType = p.Division_Type,
                    PhoneCode = p.Phone_Code
                };

                foreach (var w in p.Wards)
                {
                    province.Wards.Add(new Ward
                    {
                        Code = w.Code,
                        Name = w.Name,
                        Codename = w.Codename,
                        DivisionType = w.Division_Type,
                        ShortCodename = w.Short_Codename
                    });
                }

                _db.Provinces.Add(province);
            }

            await _db.SaveChangesAsync();
        }
    }
}