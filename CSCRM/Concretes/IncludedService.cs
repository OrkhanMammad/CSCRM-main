using CSCRM.Abstractions;
using CSCRM.DataAccessLayers;
using CSCRM.Models;
using CSCRM.Models.ResponseTypes;
using CSCRM.ViewModels.HotelVMs;
using CSCRM.ViewModels.InclusivesVMs;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;

namespace CSCRM.Concretes
{
    public class IncludedService : IIncludedService
    {
        readonly AppDbContext _context;
        public IncludedService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<BaseResponse> AddInclusiveAsync(AddNewInclusiveVM inclusiveVM, AppUser appUser)
        {
            try
            {

                if (inclusiveVM == null || string.IsNullOrEmpty(inclusiveVM.Name))
                {
                    List<GetInclusive> inclusivesInDb = await _context.InclusiveServices
                                                                             .Where(i => i.IsDeleted == false)
                                                                             .Select(i => new GetInclusive
                                                                             {
                                                                                 Id = i.Id,
                                                                                 Name = i.Name,
                                                                                 Price = i.Price,
                                                                             }).ToListAsync();
                    return new BaseResponse
                    {
                        Message = $"Service Name can not be empty",
                        StatusCode = "201",
                        Success = true,
                        Data = inclusivesInDb
                    };
                }

                var inclusivesNamesInDB = await _context.InclusiveServices.Where(h => h.IsDeleted == false).Select(h => h.Name).ToListAsync();
                if (inclusivesNamesInDB.Any(hn => hn.ToLower() == inclusiveVM.Name.Trim().ToLower()))
                {
                    List<GetInclusive> inclusivessInDb = await _context.InclusiveServices
                                                                             .Where(i => i.IsDeleted == false)
                                                                             .Select(i => new GetInclusive
                                                                             {
                                                                                 Id = i.Id,
                                                                                 Name = i.Name,
                                                                                 Price = i.Price,
                                                                             }).ToListAsync();


                    return new BaseResponse
                    {
                        Message = $"Service {inclusiveVM.Name} is already exists",
                        StatusCode = "409",
                        Success = false,
                        Data = inclusivessInDb,

                    };

                }

                Inclusive newInclusive = new Inclusive
                {
                    Name = inclusiveVM.Name.Trim(),
                    Price = inclusiveVM.Price,
                    CreatedBy = appUser.Name + " " + appUser.SurName
                };
                await _context.InclusiveServices.AddAsync(newInclusive);
                await _context.SaveChangesAsync();
                List<GetInclusive> inclusives = await _context.InclusiveServices
                                                                             .Where(i => i.IsDeleted == false)
                                                                             .Select(i => new GetInclusive
                                                                             {
                                                                                 Id = i.Id,
                                                                                 Name = i.Name,
                                                                                 Price = i.Price,
                                                                             }).ToListAsync();

                return new BaseResponse
                {
                    Data = inclusives,
                    Message = "Service Created Successfully",
                    StatusCode = "201",
                    Success = true,
                };


            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Message = "Service Could Not Created Successfully, Unhandled error occured",
                    StatusCode = "500",
                    Success = false,
                    Data = new List<GetInclusive>()
                };
            }
        }
        public async Task<BaseResponse> EditInclusiveAsync(EditInclusiveVM inclusive, AppUser appUser)
        {
            if (string.IsNullOrWhiteSpace(inclusive.Name))
            {
                return new BaseResponse
                {
                    Success = false,
                    Message = "Service name cannot be empty.",
                    StatusCode = "400",
                    Data = inclusive
                };
            }
            if (inclusive == null || inclusive.Id <= 0)
            {
                return new BaseResponse
                {
                    Success = false,
                    Message = "Invalid service ID.",
                    StatusCode = "400",
                    Data = inclusive
                };
            }

            try
            {
                bool inclusiveExists = await _context.InclusiveServices.AnyAsync(h => h.Name.ToLower() == inclusive.Name.ToLower().Trim()
                                                                     && h.IsDeleted == false
                                                                     && h.Id != inclusive.Id);

                if (inclusiveExists)
                {
                    return new BaseResponse
                    {
                        Message = $"Service {inclusive.Name} is already exists",
                        StatusCode = "409",
                        Success = false,
                        Data = inclusive
                    };
                }

                Inclusive editInclusive = await _context.InclusiveServices.FirstOrDefaultAsync(h => h.Id == inclusive.Id);
                if (editInclusive == null)
                {
                    return new BaseResponse
                    {
                        Success = false,
                        Message = "Service not found.",
                        StatusCode = "404",
                        Data = inclusive
                    };
                }




                string userNmSrnm = appUser.Name + " " + appUser.SurName;

                editInclusive.Name = inclusive.Name.Trim();
                editInclusive.Price=inclusive.Price;
                editInclusive.UpdatedBy = userNmSrnm;
                await _context.SaveChangesAsync();


                Inclusive inclusiveEntity = await _context.InclusiveServices
                                                       .FirstOrDefaultAsync(h => h.IsDeleted == false && h.Id == editInclusive.Id);

                EditInclusiveVM inclusiveEdited = new EditInclusiveVM
                {
                    Id = inclusiveEntity.Id,
                    Name = inclusiveEntity.Name,
                    Price = inclusiveEntity.Price,
                   
                };

                return new BaseResponse
                {
                    Data = inclusiveEdited,
                    Message = "Service updated successfully.",
                    Success = true,
                    StatusCode = "200"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Data = inclusive,
                    Success = false,
                    Message = "An unhandled exception occurred.",
                    StatusCode = "500"
                };
            }
        } 
        public async Task<BaseResponse> GetAllInclusivesAsync()
        {
            try
            {
                List<GetInclusive> inclusiveServices = await _context.InclusiveServices
                                                                            .Where(i => i.IsDeleted == false)
                                                                            .Select(i => new GetInclusive
                                                                            {
                                                                                Id = i.Id,
                                                                                Name = i.Name,
                                                                                Price = i.Price,
                                                                            }).ToListAsync();
                return inclusiveServices.Any()
                ? new BaseResponse { Data = inclusiveServices, Success = true, StatusCode = "201" }
                : new BaseResponse { Data = new List<GetInclusive>(), Message = "No Inclusive Service Found", Success = true, StatusCode = "200" };
            }
            catch (Exception ex)
            {

                return new BaseResponse
                {
                    StatusCode = "500",
                    Message = "Unhandled Error Occured",
                    Success = false,
                    Data = new List<GetInclusive>()
                };
            }
        }
        public async Task<BaseResponse> GetInclusiveByIdAsync(int inclusiveId)
        {
            try
            {
                Inclusive inclusiveEntity = await _context.InclusiveServices.FirstOrDefaultAsync(h => h.IsDeleted == false && h.Id == inclusiveId);
                if (inclusiveEntity == null)
                {
                    return new BaseResponse
                    {
                        Message = "Service Could Not Found",
                        StatusCode = "404",
                        Success = false,
                        Data = new EditInclusiveVM()
                    };
                }

                EditInclusiveVM inclusiveForEdit = new EditInclusiveVM
                {
                    Id = inclusiveEntity.Id,
                    Name = inclusiveEntity.Name,
                    Price = inclusiveEntity.Price,
                    
                };
                return new BaseResponse
                {
                    Success = true,
                    Data = inclusiveForEdit,
                    StatusCode = "201"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Success = false,
                    Data = new EditInclusiveVM(),
                    StatusCode = "500",
                    Message = "Unhandled error occured"
                };
            }
        }
        public async Task<BaseResponse> RemoveInclusiveAsync(int inclusiveId, AppUser appUser)
        {
            try
            {
                Inclusive deletingInclusive = await _context.InclusiveServices.FirstOrDefaultAsync(h => h.Id == inclusiveId && h.IsDeleted == false);
                if (deletingInclusive == null)
                {
                    List<GetInclusive> inclusiveServices = await _context.InclusiveServices
                                                                             .Where(i => i.IsDeleted == false)
                                                                             .Select(i => new GetInclusive
                                                                             {
                                                                                 Id = i.Id,
                                                                                 Name = i.Name,
                                                                                 Price = i.Price,
                                                                             }).ToListAsync();
                    return new BaseResponse
                    {
                        Success = false,
                        Message = "Service Could Not Found",
                        StatusCode = "404",
                        Data = inclusiveServices,                       
                    };
                }

                deletingInclusive.IsDeleted = true;
                deletingInclusive.DeletedBy = appUser.Name + " " + appUser.SurName;
                await _context.SaveChangesAsync();
                List<GetInclusive> inclusiveServicesInDb = await _context.InclusiveServices
                                                                             .Where(i => i.IsDeleted == false)
                                                                             .Select(i => new GetInclusive
                                                                             {
                                                                                 Id = i.Id,
                                                                                 Name = i.Name,
                                                                                 Price = i.Price,
                                                                             }).ToListAsync();
                return new BaseResponse
                {
                    Success = true,
                    Message = $"Service {deletingInclusive.Name} is deleted successfully.",
                    Data = inclusiveServicesInDb,
                    StatusCode = "203",
              
                };
            }

            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Success = false,
                    StatusCode = "500",
                    Message = "Service Could Not Deleted Successfully",
                    Data = new List<GetInclusive>()
                };
            }
        }
    }
}
