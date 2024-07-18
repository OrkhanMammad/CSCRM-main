using CSCRM.Abstractions;
using CSCRM.DataAccessLayers;
using CSCRM.Models;
using CSCRM.Models.ResponseTypes;
using CSCRM.ViewModels.CompanyVMs;
using CSCRM.ViewModels.HotelVMs;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CSCRM.Concretes
{
    public class CompanyService : ICompanyService
    {
        readonly AppDbContext _context;
        public CompanyService(AppDbContext context)
        {
                _context = context;
        }

        private void CompanyEditor(Company company, EditCompanyVM updatedCompany, string userNmSrnm)
        {
            company.Name = updatedCompany.Name;
            company.Address = updatedCompany.Address;
            company.ContactPerson = updatedCompany.ContactPerson;
            company.Email = updatedCompany.Email;
            company.Phone = updatedCompany.Phone;
            company.UpdatedBy = userNmSrnm;
        }

        private async Task<List<GetCompanyVM>> GetCompaniesAsync(int pageIndex)
        {
            return await _context.Companies
                                           .Where(h => h.IsDeleted == false)
                                           .OrderByDescending(h => h.Id)
                                           .Skip((pageIndex - 1) * 6)
                                           .Take(6)
                                           .Select(h => new GetCompanyVM
                                                        {
                                                            Id = h.Id,
                                                            Name = h.Name,
                                                            ContactPerson = h.ContactPerson,
                                                            Phone = h.Phone,
                                                            Email = h.Email,
                                                            Address = h.Address,
                                                        })
                                           .ToListAsync();

        }
        public async Task<BaseResponse> AddCompanyAsync(AddCompanyVM companyVM, AppUser appUser)
        {
            try
            {

                if (string.IsNullOrEmpty(companyVM.Name))
                {
                    List<GetCompanyVM> companiesInDb = await GetCompaniesAsync(1);
                    int companyCount = await _context.Companies.CountAsync(ct => ct.IsDeleted == false);
                    int pageSize = (int)Math.Ceiling((decimal)companyCount / 6);
                    return new BaseResponse 
                    { 
                        Message = $"Company Name can not be empty", 
                        StatusCode = "400", 
                        Success = false, 
                        Data = companiesInDb,
                        PageSize = pageSize,
                        PageIndex=1
                    };
                }

                bool companyExists = await _context.Companies.AnyAsync(h => h.IsDeleted == false && h.Name.ToLower() == companyVM.Name.Trim().ToLower());
                
                if (companyExists)
                {
                    List<GetCompanyVM> companiesInDb = await GetCompaniesAsync(1);
                    int companyCount = await _context.Companies.CountAsync(ct => ct.IsDeleted == false);
                    int pageSize = (int)Math.Ceiling((decimal)companyCount / 6);
                    return new BaseResponse 
                    { 
                        Message = $"Company {companyVM.Name} is already exists", 
                        StatusCode = "409", 
                        Success = false, 
                        Data=companiesInDb,
                        PageSize = pageSize,
                        PageIndex=1
                    };
                }

                Company newCompany = new Company
                {
                    Name = companyVM.Name.Trim(),
                    ContactPerson = companyVM.ContactPerson,
                    Address = companyVM.Address.Trim(),
                    Email = companyVM.Email.Trim(),
                    Phone = companyVM.Phone.Trim(),
                    CreatedBy = appUser.Name + " " + appUser.SurName,
                };
                await _context.Companies.AddAsync(newCompany);
                await _context.SaveChangesAsync();
                List<GetCompanyVM> companies = await GetCompaniesAsync(1);
                int companyCountInDb = await _context.Companies.CountAsync(ct => ct.IsDeleted == false);
                int pageSizeForCompanies = (int)Math.Ceiling((decimal)companyCountInDb / 6);
                return new BaseResponse 
                { 
                    Data = companies, 
                    Message = "Company Created Successfully",
                    StatusCode = "201", 
                    Success = true,
                    PageIndex = 1,
                    PageSize = pageSizeForCompanies
                };


            }
            catch (Exception ex)
            {
                return new BaseResponse 
                { 
                    Message = "Company Could Not Created Successfully, Unhadled error occured", 
                    StatusCode = "500", 
                    Success = false, 
                    Data=new List<GetCompanyVM>() 
                };
            }
        }
        public async Task<BaseResponse> GetAllCompaniesAsync(int pageIndex)
        {
            try
            {
                List<GetCompanyVM> companies = await GetCompaniesAsync(pageIndex);
                int companyCount = await _context.Companies.CountAsync(ct => ct.IsDeleted == false);
                int pageSize = (int)Math.Ceiling((decimal)companyCount / 6);
                return companies.Any()
                ? new BaseResponse { Data = companies, Success = true, StatusCode = "200", PageIndex=pageIndex, PageSize=pageSize }
                : new BaseResponse { Data = new List<GetCompanyVM>(), Message = "No company found", Success = true, StatusCode = "200" };

            }
            catch (Exception ex)
            {
                return new BaseResponse 
                { 
                    StatusCode = "404", 
                    Message = "Unhandled error occured", 
                    Success = false, 
                    Data=new List<GetCompanyVM>() 
                };
            }
        }
        public async Task<BaseResponse> RemoveCompanyAsync(int companyId, AppUser appUser)
        {
            try
            {
                Company deletingCompany = await _context.Companies.FirstOrDefaultAsync(h => h.Id == companyId && h.IsDeleted == false);
                if (deletingCompany == null)
                    return new BaseResponse 
                    { 
                        Success = false, 
                        Message = "Company Could Not Found", 
                        StatusCode = "404", 
                        Data= new List<GetCompanyVM>() 
                    }; 

                deletingCompany.IsDeleted = true;
                deletingCompany.DeletedBy = appUser.Name + " " + appUser.SurName;
                await _context.SaveChangesAsync();
                List<GetCompanyVM> companies = await GetCompaniesAsync(1);
                int companyCount = await _context.Companies.CountAsync(ct => ct.IsDeleted == false);
                int pageSize = (int)Math.Ceiling((decimal)companyCount / 6);
                return new BaseResponse 
                { 
                    Success = true, 
                    Message = $"Company {deletingCompany.Name} is deleted successfully.", 
                    Data = companies,
                    PageSize = pageSize,
                    PageIndex = 1
                };
            }

            catch (Exception ex) 
            { 
                return new BaseResponse 
                { 
                    Success = false, 
                    StatusCode = "500", 
                    Message = "Company Could Not Deleted Successfully", 
                    Data= new List<GetCompanyVM>() 
                }; 
            }

        }
        public async Task<BaseResponse> GetCompanyByIdAsync(int companyId)
        {
            try
            {
                Company companyEntity = await _context.Companies.FirstOrDefaultAsync(h => h.IsDeleted == false && h.Id == companyId);
                if (companyEntity == null)
                return new BaseResponse 
                { 
                    Message = "Company Could Not Found", 
                    StatusCode = "404", 
                    Success = false, 
                    Data = new EditCompanyVM() 
                };
                

                EditCompanyVM companyForEdit = new EditCompanyVM
                {
                   Id=companyEntity.Id,
                   Name = companyEntity.Name,
                   ContactPerson = companyEntity.ContactPerson,
                   Address = companyEntity.Address,
                   Email = companyEntity.Email,
                   Phone = companyEntity.Phone                  
                };
                return new BaseResponse 
                { 
                    Success = true, 
                    Data = companyForEdit, 
                    StatusCode = "201" 
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse 
                { 
                    Success = false, 
                    Data = new EditCompanyVM(), 
                    StatusCode = "500", 
                    Message = "Unhandled error occured" 
                };
            }
        }
        public async Task<BaseResponse> EditCompanyAsync(EditCompanyVM company, AppUser appUser)
        {
            if (company == null || company.Id <= 0)
                return new BaseResponse
                {
                    Success = false,
                    Message = "Invalid company ID.",
                    StatusCode = "400",
                    Data = company
                };


            if (string.IsNullOrWhiteSpace(company.Name))
                return new BaseResponse
                {
                    Success = false,
                    Message = "Company name cannot be empty.",
                    StatusCode = "400",
                    Data = company
                };


            try
            {

                bool companyExists = await _context.Companies.AnyAsync(h => h.IsDeleted == false 
                                                                            && h.Name.ToLower() == company.Name.Trim().ToLower()
                                                                            && h.Id != company.Id);
                if (companyExists)
                {                   
                    return new BaseResponse 
                    { 
                        Message = $"Company {company.Name} is already exists", 
                        StatusCode = "409",   
                        Success = false, 
                        Data = company 

                    };
                }

                Company editCompany = await _context.Companies.FirstOrDefaultAsync(c => c.Id == company.Id);
                if (editCompany == null)
                    return new BaseResponse
                    {
                        Success = false,
                        Message = "Company not found.",
                        StatusCode = "404",
                        Data = company
                    };
                
                

                string userNmSrnm = appUser.Name + " " + appUser.SurName;
                CompanyEditor(editCompany, company, userNmSrnm);
                await _context.SaveChangesAsync();


                Company companyEntity = await _context.Companies
                                                                .FirstOrDefaultAsync(h => h.IsDeleted == false && h.Id == editCompany.Id);

                EditCompanyVM companyEdited = new EditCompanyVM
                {
                    Id = companyEntity.Id,
                    Name = companyEntity.Name,
                    ContactPerson = companyEntity.ContactPerson,
                    Address = companyEntity.Address,
                    Email = companyEntity.Email,
                    Phone = companyEntity.Phone,
                };

                return new BaseResponse
                {
                    Data = companyEdited,
                    Message = "Company updated successfully.",
                    Success = true,
                    StatusCode = "203"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {   
                    Data = new EditCompanyVM(),
                    Success = false,
                    Message = "An unhandled exception occurred.",
                    StatusCode = "500"
                };
            }
        }


    }
}
