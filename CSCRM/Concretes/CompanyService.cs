using CSCRM.Abstractions;
using CSCRM.DataAccessLayers;
using CSCRM.Models;
using CSCRM.Models.ResponseTypes;
using CSCRM.ViewModels.CompanyVMs;
using CSCRM.ViewModels.HotelVMs;
using Microsoft.EntityFrameworkCore;

namespace CSCRM.Concretes
{
    public class CompanyService : ICompanyService
    {
        readonly AppDbContext _context;
        public CompanyService(AppDbContext context)
        {
                _context = context;
        }

        private void CompanyEditor(Company company, EditCompanyVM updatedCompany)
        {
           company.Name = updatedCompany.Name;
            company.Address = updatedCompany.Address;
            company.Email = updatedCompany.Email;
            company.Phone = updatedCompany.Phone;
        }

        public async Task<BaseResponse> AddCompanyAsync(AddCompanyVM companyVM)
        {
            try
            {
                if (string.IsNullOrEmpty(companyVM.Name))
                {
                    List<GetCompanyVM> companiesInDb = await _context.Companies
                                                        .Where(h => h.IsDeleted == false)
                                                        .Select(h => new GetCompanyVM
                                                        {
                                                            Id = h.Id,
                                                            Name = h.Name,
                                                            Phone = h.Phone,
                                                            Email = h.Email,
                                                            Address = h.Address,
                                                        })
                                                        .ToListAsync();
                    return new BaseResponse { Message = $"Company Name can not be empty", StatusCode = "201", Success = false, Data = companiesInDb };

                }

                List<string> companyNamesInDB = await _context.Companies.Where(h => h.IsDeleted == false).Select(h => h.Name).ToListAsync();
                if (companyNamesInDB.Any(hn => hn.ToLower() == companyVM.Name.Trim().ToLower()))
                {
                    List<GetCompanyVM> companiesInDb = await _context.Companies
                                                        .Where(h => h.IsDeleted == false)
                                                        .Select(h => new GetCompanyVM
                                                        {
                                                            Id = h.Id,
                                                            Name = h.Name,
                                                            Phone = h.Phone,
                                                            Email = h.Email,
                                                            Address = h.Address,
                                                        })
                                                        .ToListAsync();
                    return new BaseResponse { Message = $"Company {companyVM.Name} is already exists", StatusCode = "201", Success = false, Data=companiesInDb };
                }

                Company newCompany = new Company
                {
                    Name = companyVM.Name,
                    Address = companyVM.Address,
                    Email = companyVM.Email,
                    Phone = companyVM.Phone                   
                };
                await _context.Companies.AddAsync(newCompany);
                await _context.SaveChangesAsync();
                List<GetCompanyVM> companies = await _context.Companies
                                                        .Where(h => h.IsDeleted == false)
                                                        .Select(h => new GetCompanyVM
                                                        {
                                                            Id = h.Id,
                                                            Name = h.Name,
                                                            Phone = h.Phone,
                                                            Email = h.Email,
                                                            Address= h.Address,
                                                        })
                                                        .ToListAsync();
                return new BaseResponse { Data = companies, Message = "Company Created Successfully", StatusCode = "201", Success = true };


            }
            catch (Exception ex)
            {
                return new BaseResponse { Message = "Company Could Not Created Successfully, Unhadled error occured", StatusCode = "500", Success = false };

            }
        }

        public async Task<BaseResponse> GetAllCompaniesAsync()
        {
            try
            {
                List<GetCompanyVM> companies = await _context.Companies
                                                        .Where(c => c.IsDeleted == false)
                                                        .Select(c => new GetCompanyVM
                                                        {
                                                            Id = c.Id,
                                                            Name = c.Name,
                                                            Address = c.Address,
                                                            Email = c.Email,
                                                            Phone = c.Phone                                                            
                                                        })
                                                        .ToListAsync();


                if (companies.Count() == 0)
                {
                    return new BaseResponse { Data = new List<GetHotelVM>(), Message = "No company found", Success = true, StatusCode = "200" };
                }
                else
                {
                    return new BaseResponse { Data = companies, Success = true, StatusCode = "201" };

                }

            }
            catch (Exception ex)
            {
                return new BaseResponse { StatusCode = "404", Message = "Unhandled error occured", Success = false };
            }


        }

        public async Task<BaseResponse> RemoveCompanyAsync(int companyId)
        {
            try
            {
                Company deletingCompany = await _context.Companies.FirstOrDefaultAsync(h => h.Id == companyId && h.IsDeleted == false);
                if (deletingCompany == null) { return new BaseResponse { Success = false, Message = "Company Could Not Found", StatusCode = "404" }; }

                deletingCompany.IsDeleted = true;
                await _context.SaveChangesAsync();
                List<GetCompanyVM> companies = await _context.Companies
                                                       .Where(h => h.IsDeleted == false)
                                                       .Select(h => new GetCompanyVM
                                                       {
                                                           Id = h.Id,
                                                           Name = h.Name,
                                                          Address = h.Address,
                                                          Email = h.Email,
                                                          Phone = h.Phone
                                                       })
                                                       .ToListAsync();

                return new BaseResponse { Success = true, Message = $"Company {deletingCompany.Name} is deleted successfully.", Data = companies };
            }

            catch (Exception ex) { return new BaseResponse { Success = false, StatusCode = "500", Message = "Company Could Not Deleted Successfully" }; }



        }

        public async Task<BaseResponse> GetCompanyByIdAsync(int companyId)
        {
            try
            {
                Company companyEntity = await _context.Companies.FirstOrDefaultAsync(h => h.IsDeleted == false && h.Id == companyId);
                if (companyEntity == null)
                {
                    return new BaseResponse { Message = "Company Could Not Found", StatusCode = "404", Success = false, Data = new EditCompanyVM() };
                }

                EditCompanyVM companyForEdit = new EditCompanyVM
                {
                   Id=companyEntity.Id,
                   Name = companyEntity.Name,
                   Address = companyEntity.Address,
                   Email = companyEntity.Email,
                   Phone = companyEntity.Phone                  
                };
                return new BaseResponse { Success = true, Data = companyForEdit, StatusCode = "201" };
            }
            catch (Exception ex)
            {
                return new BaseResponse { Success = false, Data = new EditCompanyVM(), StatusCode = "500", Message = "Unhandled error occured" };
            }
        }


        public async Task<BaseResponse> EditCompanyAsync(EditCompanyVM company)
        {
            try
            {

                if (company == null || company.Id <= 0)
                {
                    return new BaseResponse
                    {
                        Success = false,
                        Message = "Invalid company ID.",
                        StatusCode = "400",
                        Data = company
                    };
                }


                Company editCompany = await _context.Companies.FirstOrDefaultAsync(c => c.Id == company.Id);
                if (editCompany == null)
                {
                    return new BaseResponse
                    {
                        Success = false,
                        Message = "Company not found.",
                        StatusCode = "404",
                        Data = company
                    };
                }


                if (string.IsNullOrWhiteSpace(company.Name))
                {
                    return new BaseResponse
                    {
                        Success = false,
                        Message = "Company name cannot be empty.",
                        StatusCode = "400",
                        Data = company
                    };
                }


                CompanyEditor(editCompany, company);
                await _context.SaveChangesAsync();


                Company companyEntity = await _context.Companies
                                                       .FirstOrDefaultAsync(h => h.IsDeleted == false && h.Id == editCompany.Id);

                EditCompanyVM companyEdited = new EditCompanyVM
                {
                    Id = companyEntity.Id,
                    Name = companyEntity.Name,
                    Address = companyEntity.Address,
                    Email = companyEntity.Email,
                    Phone = companyEntity.Phone,
                };

                return new BaseResponse
                {
                    Data = companyEdited,
                    Message = "Company updated successfully.",
                    Success = true,
                    StatusCode = "200"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {  Data = new EditCompanyVM(),
                    Success = false,
                    Message = "An unhandled exception occurred.",
                    StatusCode = "500"
                };
            }
        }


    }
}
