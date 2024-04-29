using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Northwind.EntityModels;
using System.Security.Cryptography.X509Certificates;

namespace Northwind.Web.Pages
{
    public class SuppliersModel : PageModel
    {
        private NorthwindContext _db;
        public SuppliersModel(NorthwindContext db)
        {
            _db = db;
        }

        [BindProperty]
        public Supplier? Supplier { get; set; }

        public IEnumerable<Supplier>? Suppliers {  get; set; } 

        public void OnGet()
        {
            ViewData["Title"] = "Northwind B2B - Suppliers";

            Suppliers = _db.Suppliers
                .OrderBy(c => c.Country)
                .ThenBy(c => c.CompanyName);
        }

        public IActionResult OnPost()
        {
            if(Supplier is not null && ModelState.IsValid)
            {
                _db.Suppliers.Add(Supplier);
                _db.SaveChanges();
                return RedirectToPage("/suppliers");
            }
            else
            {
                return Page();
            }
        }
    }
}
