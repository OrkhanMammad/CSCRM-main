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
        private readonly ILogger<IncludedService> _logger;
        public IncludedService(AppDbContext context, ILogger<IncludedService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<BaseResponse> AddInclusiveAsync(AddNewInclusiveVM inclusiveVM, AppUser appUser)
        {
            try
            {
                if (inclusiveVM == null || string.IsNullOrEmpty(inclusiveVM.Name))
                {
                    _logger.LogWarning("Attempted to add a new inclusive service with an empty name.");
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
                        Message = "Service Name cannot be empty.",
                        StatusCode = "400",
                        Success = false,
                        Data = inclusivesInDb
                    };
                }

                _logger.LogInformation("Checking if inclusive service name '{ServiceName}' already exists.", inclusiveVM.Name);
                var inclusivesNamesInDB = await _context.InclusiveServices
                    .Where(h => h.IsDeleted == false)
                    .Select(h => h.Name)
                    .ToListAsync();

                if (inclusivesNamesInDB.Any(hn => hn.ToLower() == inclusiveVM.Name.Trim().ToLower()))
                {
                    _logger.LogWarning("Inclusive service with name '{ServiceName}' already exists.", inclusiveVM.Name);
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
                        Message = $"Service {inclusiveVM.Name} already exists.",
                        StatusCode = "409",
                        Success = false,
                        Data = inclusivesInDb
                    };
                }

                Inclusive newInclusive = new Inclusive
                {
                    Name = inclusiveVM.Name.Trim(),
                    Price = inclusiveVM.Price,
                    CreatedBy = appUser.Name + " " + appUser.SurName
                };
                _logger.LogInformation("Adding new inclusive service '{ServiceName}' by user '{UserName}'.", newInclusive.Name, appUser.Name);
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

                _logger.LogInformation("Inclusive service '{ServiceName}' created successfully.", newInclusive.Name);

                return new BaseResponse
                {
                    Data = inclusives,
                    Message = "Service Created Successfully",
                    StatusCode = "201",
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the inclusive service '{ServiceName}'.", inclusiveVM?.Name);
                return new BaseResponse
                {
                    Message = "Service could not be created successfully. Unhandled error occurred.",
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
                _logger.LogWarning("Attempted to edit inclusive service with an empty name.");
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
                _logger.LogWarning("Attempted to edit inclusive service with invalid ID: {Id}.", inclusive?.Id);
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
                _logger.LogInformation("Checking if inclusive service name '{ServiceName}' already exists.", inclusive.Name);
                bool inclusiveExists = await _context.InclusiveServices.AnyAsync(h => h.Name.ToLower() == inclusive.Name.ToLower().Trim()
                                                                   && h.IsDeleted == false
                                                                   && h.Id != inclusive.Id);

                if (inclusiveExists)
                {
                    _logger.LogWarning("Inclusive service with name '{ServiceName}' already exists.", inclusive.Name);
                    return new BaseResponse
                    {
                        Message = $"Service {inclusive.Name} already exists.",
                        StatusCode = "409",
                        Success = false,
                        Data = inclusive
                    };
                }

                _logger.LogInformation("Fetching inclusive service with ID {Id}.", inclusive.Id);
                Inclusive editInclusive = await _context.InclusiveServices.FirstOrDefaultAsync(h => h.Id == inclusive.Id);
                if (editInclusive == null)
                {
                    _logger.LogWarning("Inclusive service with ID {Id} not found.", inclusive.Id);
                    return new BaseResponse
                    {
                        Success = false,
                        Message = "Service not found.",
                        StatusCode = "404",
                        Data = inclusive
                    };
                }

                string userNmSrnm = appUser.Name + " " + appUser.SurName;

                _logger.LogInformation("Updating inclusive service with ID {Id} by user '{UserName}'.", inclusive.Id, userNmSrnm);
                editInclusive.Name = inclusive.Name.Trim();
                editInclusive.Price = inclusive.Price;
                editInclusive.UpdatedBy = userNmSrnm;
                await _context.SaveChangesAsync();

                Inclusive inclusiveEntity = await _context.InclusiveServices
                    .FirstOrDefaultAsync(h => h.IsDeleted == false && h.Id == editInclusive.Id);

                EditInclusiveVM inclusiveEdited = new EditInclusiveVM
                {
                    Id = inclusiveEntity.Id,
                    Name = inclusiveEntity.Name,
                    Price = inclusiveEntity.Price
                };

                _logger.LogInformation("Inclusive service with ID {Id} updated successfully.", inclusive.Id);

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
                _logger.LogError(ex, "An error occurred while updating inclusive service with ID {Id}.", inclusive?.Id);
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

                if (inclusiveServices.Any())
                {
                    _logger.LogInformation("Retrieved all inclusive services successfully.");
                    return new BaseResponse
                    {
                        Data = inclusiveServices,
                        Success = true,
                        StatusCode = "200" // Use 200 OK for successful data retrieval
                    };
                }
                else
                {
                    _logger.LogInformation("No inclusive services found.");
                    return new BaseResponse
                    {
                        Data = new List<GetInclusive>(),
                        Message = "No Inclusive Service Found",
                        Success = true,
                        StatusCode = "200" // Use 200 OK even when no data is found
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled error occurred while retrieving inclusive services.");
                return new BaseResponse
                {
                    StatusCode = "500",
                    Message = "Unhandled Error Occurred",
                    Success = false,
                    Data = new List<GetInclusive>()
                };
            }
        }

        public async Task<BaseResponse> GetInclusiveByIdAsync(int inclusiveId)
        {
            try
            {
                Inclusive inclusiveEntity = await _context.InclusiveServices
                    .FirstOrDefaultAsync(h => h.IsDeleted == false && h.Id == inclusiveId);

                if (inclusiveEntity == null)
                {
                    return new BaseResponse
                    {
                        Message = "Service not found.",
                        StatusCode = "404",
                        Success = false,
                        Data = new EditInclusiveVM()
                    };
                }

                EditInclusiveVM inclusiveForEdit = new EditInclusiveVM
                {
                    Id = inclusiveEntity.Id,
                    Name = inclusiveEntity.Name,
                    Price = inclusiveEntity.Price
                };

                return new BaseResponse
                {
                    Success = true,
                    Data = inclusiveForEdit,
                    StatusCode = "200" // Use 200 OK for successful data retrieval
                };
            }
            catch (Exception ex)
            {
                // Optionally, log the exception for troubleshooting
                _logger.LogError(ex, "An unhandled error occurred while retrieving the inclusive service.");

                return new BaseResponse
                {
                    Success = false,
                    Data = new EditInclusiveVM(),
                    StatusCode = "500",
                    Message = "An unhandled error occurred while retrieving the service details."
                };
            }
        }

        public async Task<BaseResponse> RemoveInclusiveAsync(int inclusiveId, AppUser appUser)
        {
            try
            {
                _logger.LogInformation($"Attempting to remove inclusive service with ID {inclusiveId}.");

                Inclusive deletingInclusive = await _context.InclusiveServices
                    .FirstOrDefaultAsync(h => h.Id == inclusiveId && h.IsDeleted == false);

                if (deletingInclusive == null)
                {
                    _logger.LogWarning($"Inclusive service with ID {inclusiveId} not found.");

                    List<GetInclusive> inclusiveServices = await _context.InclusiveServices
                        .Where(i => i.IsDeleted == false)
                        .Select(i => new GetInclusive
                        {
                            Id = i.Id,
                            Name = i.Name,
                            Price = i.Price
                        }).ToListAsync();

                    return new BaseResponse
                    {
                        Success = false,
                        Message = "Service not found.",
                        StatusCode = "404",
                        Data = inclusiveServices
                    };
                }

                deletingInclusive.IsDeleted = true;
                deletingInclusive.DeletedBy = $"{appUser.Name} {appUser.SurName}";

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Inclusive service with ID {inclusiveId} marked as deleted by {appUser.Name} {appUser.SurName}.");

                List<GetInclusive> inclusiveServicesInDb = await _context.InclusiveServices
                    .Where(i => i.IsDeleted == false)
                    .Select(i => new GetInclusive
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Price = i.Price
                    }).ToListAsync();

                return new BaseResponse
                {
                    Success = true,
                    Message = $"Service {deletingInclusive.Name} deleted successfully.",
                    Data = inclusiveServicesInDb,
                    StatusCode = "200" // Changed status code to 200 for successful deletion
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while trying to delete inclusive service with ID {inclusiveId}.");

                return new BaseResponse
                {
                    Success = false,
                    StatusCode = "500",
                    Message = "Failed to delete the service due to an unhandled error.",
                    Data = new List<GetInclusive>()
                };
            }
        }

    }
}
