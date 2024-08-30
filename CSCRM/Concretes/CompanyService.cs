using CSCRM.Abstractions;
using CSCRM.dataAccessLayers;
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
        private readonly ILogger<CompanyService> _logger;
        public CompanyService(AppDbContext context, ILogger<CompanyService> logger)
        {
            _context = context;
            _logger = logger;
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

                    _logger.LogWarning("Attempt to add a company failed due to empty company name.");

                    return new BaseResponse
                    {
                        Message = "Company Name can not be empty",
                        StatusCode = "400",
                        Success = false,
                        data = companiesInDb,
                        PageSize = pageSize,
                        PageIndex = 1
                    };
                }

                bool companyExists = await _context.Companies.AnyAsync(h => h.IsDeleted == false && h.Name.ToLower() == companyVM.Name.Trim().ToLower());

                if (companyExists)
                {
                    List<GetCompanyVM> companiesInDb = await GetCompaniesAsync(1);
                    int companyCount = await _context.Companies.CountAsync(ct => ct.IsDeleted == false);
                    int pageSize = (int)Math.Ceiling((decimal)companyCount / 6);

                    _logger.LogWarning("Attempt to add a company failed because company {CompanyName} already exists.", companyVM.Name);

                    return new BaseResponse
                    {
                        Message = $"Company {companyVM.Name} already exists",
                        StatusCode = "409",
                        Success = false,
                        data = companiesInDb,
                        PageSize = pageSize,
                        PageIndex = 1
                    };
                }

                Company newCompany = new Company
                {
                    Name = companyVM.Name.Trim(),
                    ContactPerson = companyVM.ContactPerson,
                    Address = companyVM.Address.Trim(),
                    Email = companyVM.Email.Trim(),
                    Phone = companyVM.Phone.Trim(),
                    CreatedBy = $"{appUser.Name} {appUser.SurName}",
                };
                await _context.Companies.AddAsync(newCompany);
                await _context.SaveChangesAsync();

                List<GetCompanyVM> companies = await GetCompaniesAsync(1);
                int companyCountInDb = await _context.Companies.CountAsync(ct => ct.IsDeleted == false);
                int pageSizeForCompanies = (int)Math.Ceiling((decimal)companyCountInDb / 6);

                _logger.LogInformation("Company {CompanyName} created successfully by user {UserName}.", companyVM.Name, appUser.Name);

                return new BaseResponse
                {
                    data = companies,
                    Message = "Company Created Successfully",
                    StatusCode = "201",
                    Success = true,
                    PageIndex = 1,
                    PageSize = pageSizeForCompanies
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled error occurred while creating company {CompanyName}.", companyVM.Name);

                return new BaseResponse
                {
                    Message = "Company Could Not Be Created Successfully, Unhandled error occurred",
                    StatusCode = "500",
                    Success = false,
                    data = new List<GetCompanyVM>()
                };
            }
        }

        public async Task<BaseResponse> GetAllCompaniesAsync(int pageIndex)
        {
            try
            {
                _logger.LogInformation("Fetching companies for page index {PageIndex}.", pageIndex);

                List<GetCompanyVM> companies = await GetCompaniesAsync(pageIndex);
                int companyCount = await _context.Companies.CountAsync(ct => ct.IsDeleted == false);
                int pageSize = (int)Math.Ceiling((decimal)companyCount / 6);

                if (companies.Any())
                {
                    _logger.LogInformation("Successfully fetched {CompanyCount} companies for page index {PageIndex}.", companies.Count, pageIndex);
                    return new BaseResponse
                    {
                        data = companies,
                        Success = true,
                        StatusCode = "200",
                        PageIndex = pageIndex,
                        PageSize = pageSize
                    };
                }
                else
                {
                    _logger.LogInformation("No companies found for page index {PageIndex}.", pageIndex);
                    return new BaseResponse
                    {
                        data = new List<GetCompanyVM>(),
                        Message = "No company found",
                        Success = true,
                        StatusCode = "200"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled error occurred while fetching companies for page index {PageIndex}.", pageIndex);

                return new BaseResponse
                {
                    StatusCode = "500",
                    Message = "Unhandled error occurred",
                    Success = false,
                    data = new List<GetCompanyVM>()
                };
            }
        }

        public async Task<BaseResponse> RemoveCompanyAsync(int companyId, AppUser appUser)
        {
            try
            {
                _logger.LogInformation("Attempting to delete company with ID {CompanyId}.", companyId);

                Company deletingCompany = await _context.Companies.FirstOrDefaultAsync(h => h.Id == companyId && h.IsDeleted == false);

                if (deletingCompany == null)
                {
                    _logger.LogWarning("Company with ID {CompanyId} could not be found.", companyId);
                    return new BaseResponse
                    {
                        Success = false,
                        Message = "Company Could Not Be Found",
                        StatusCode = "404",
                        data = new List<GetCompanyVM>()
                    };
                }

                deletingCompany.IsDeleted = true;
                deletingCompany.DeletedBy = $"{appUser.Name} {appUser.SurName}";
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted company with ID {CompanyId}.", companyId);

                List<GetCompanyVM> companies = await GetCompaniesAsync(1);
                int companyCount = await _context.Companies.CountAsync(ct => ct.IsDeleted == false);
                int pageSize = (int)Math.Ceiling((decimal)companyCount / 6);

                return new BaseResponse
                {
                    Success = true,
                    Message = $"Company {deletingCompany.Name} has been deleted successfully.",
                    data = companies,
                    PageSize = pageSize,
                    PageIndex = 1
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting company with ID {CompanyId}.", companyId);

                return new BaseResponse
                {
                    Success = false,
                    StatusCode = "500",
                    Message = "Company Could Not Be Deleted Successfully",
                    data = new List<GetCompanyVM>()
                };
            }
        }

        public async Task<BaseResponse> GetCompanyByIdAsync(int companyId)
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve company with ID {CompanyId}.", companyId);

                Company companyEntity = await _context.Companies.FirstOrDefaultAsync(h => h.IsDeleted == false && h.Id == companyId);

                if (companyEntity == null)
                {
                    _logger.LogWarning("Company with ID {CompanyId} could not be found.", companyId);
                    return new BaseResponse
                    {
                        Message = "Company Could Not Be Found",
                        StatusCode = "404",
                        Success = false,
                        data = new EditCompanyVM()
                    };
                }

                EditCompanyVM companyForEdit = new EditCompanyVM
                {
                    Id = companyEntity.Id,
                    Name = companyEntity.Name,
                    ContactPerson = companyEntity.ContactPerson,
                    Address = companyEntity.Address,
                    Email = companyEntity.Email,
                    Phone = companyEntity.Phone
                };

                _logger.LogInformation("Successfully retrieved company with ID {CompanyId}.", companyId);

                return new BaseResponse
                {
                    Success = true,
                    data = companyForEdit,
                    StatusCode = "200" // Changed status code to "200" for successful retrieval
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving company with ID {CompanyId}.", companyId);

                return new BaseResponse
                {
                    Success = false,
                    data = new EditCompanyVM(),
                    StatusCode = "500",
                    Message = "Unhandled error occurred"
                };
            }
        }

        public async Task<BaseResponse> EditCompanyAsync(EditCompanyVM company, AppUser appUser)
        {
            if (company == null || company.Id <= 0)
            {
                _logger.LogWarning("Attempted to edit company with invalid ID: {CompanyId}.", company?.Id);
                return new BaseResponse
                {
                    Success = false,
                    Message = "Invalid company ID.",
                    StatusCode = "400",
                    data = company
                };
            }

            if (string.IsNullOrWhiteSpace(company.Name))
            {
                _logger.LogWarning("Attempted to edit company with empty name. Company ID: {CompanyId}.", company.Id);
                return new BaseResponse
                {
                    Success = false,
                    Message = "Company name cannot be empty.",
                    StatusCode = "400",
                    data = company
                };
            }

            try
            {
                _logger.LogInformation("Attempting to update company with ID: {CompanyId}.", company.Id);

                bool companyExists = await _context.Companies.AnyAsync(h => h.IsDeleted == false
                                                                             && h.Name.ToLower() == company.Name.Trim().ToLower()
                                                                             && h.Id != company.Id);

                if (companyExists)
                {
                    _logger.LogWarning("Company with name {CompanyName} already exists. Company ID: {CompanyId}.", company.Name, company.Id);
                    return new BaseResponse
                    {
                        Message = $"Company {company.Name} already exists.",
                        StatusCode = "409",
                        Success = false,
                        data = company
                    };
                }

                Company editCompany = await _context.Companies.FirstOrDefaultAsync(c => c.Id == company.Id);
                if (editCompany == null)
                {
                    _logger.LogWarning("Company with ID {CompanyId} not found.", company.Id);
                    return new BaseResponse
                    {
                        Success = false,
                        Message = "Company not found.",
                        StatusCode = "404",
                        data = company
                    };
                }

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

                _logger.LogInformation("Company with ID {CompanyId} updated successfully.", company.Id);

                return new BaseResponse
                {
                    data = companyEdited,
                    Message = "Company updated successfully.",
                    Success = true,
                    StatusCode = "200"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred while updating company with ID {CompanyId}.", company.Id);
                return new BaseResponse
                {
                    data = new EditCompanyVM(),
                    Success = false,
                    Message = "An unhandled exception occurred.",
                    StatusCode = "500"
                };
            }
        }



    }
}
