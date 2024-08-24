using CSCRM.Abstractions;
using CSCRM.DataAccessLayers;
using CSCRM.Models;
using CSCRM.Models.ResponseTypes;
using CSCRM.ViewModels.ItineraryVMS;
using CSCRM.ViewModels.TourVMs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CSCRM.Concretes
{
    public class TourService : ITourService
    {
        readonly AppDbContext _context;
        private readonly ILogger<TourService> _logger;
        readonly UserManager<AppUser> _userManager;
        readonly ITourByCarTypeService _tourByCarTypeService;
        public TourService(AppDbContext context, UserManager<AppUser> userManager, ITourByCarTypeService service, ILogger<TourService> logger)
        {
            _context = context;
            _userManager = userManager;
            _tourByCarTypeService = service;
            _logger = logger;
        }
        private async Task<List<GetTourVM>> GetToursAsync(int pageIndex)
        {
            
                                 return await _context.Tours
                                                      .Where(c => c.IsDeleted == false)
                                                      .Include(t => t.Itineraries)
                                                      .OrderByDescending(c => c.Id)
                                                      .Skip((pageIndex-1) * 6)
                                                      .Take(6)
                                                      .Select(c => new GetTourVM
                                                      {
                                                          Id = c.Id,
                                                          Name = c.Name,
                                                          Itineraries = c.Itineraries.Select(i => new GetItineraryVM
                                                          {
                                                              Id = i.Id,
                                                              Description = i.Description,
                                                          }).ToList()
                                                      })                                                      
                                                      .ToListAsync();
        }
        public async Task<BaseResponse> GetAllToursAsync(int pageIndex)
        {
            try
            {
                _logger.LogInformation("Getting all tours for page index: {PageIndex}", pageIndex);

                // Get tours for the specified page index
                List<GetTourVM> tours = await GetToursAsync(pageIndex);
                _logger.LogInformation("Retrieved {TourCount} tours for page index: {PageIndex}", tours.Count, pageIndex);

                // Count total number of tours
                int toursCount = await _context.Tours.CountAsync(t => t.IsDeleted == false);
                _logger.LogInformation("Total number of tours (excluding deleted): {ToursCount}", toursCount);

                // Calculate page size
                int pageSize = (int)Math.Ceiling((decimal)toursCount / 6);
                _logger.LogInformation("Page size calculated as: {PageSize}", pageSize);

                // Return response
                return tours.Any()
                    ? new BaseResponse
                    {
                        Data = tours,
                        Success = true,
                        StatusCode = "200",
                        PageIndex = pageIndex,
                        PageSize = pageSize
                    }
                    : new BaseResponse
                    {
                        Data = new List<GetTourVM>(),
                        Message = "No tours found",
                        Success = true,
                        StatusCode = "200"
                    };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled error occurred while getting tours for page index: {PageIndex}", pageIndex);

                return new BaseResponse
                {
                    StatusCode = "500",
                    Message = "Unhandled error occurred",
                    Success = false,
                    Data = new List<GetTourVM>()
                };
            }
        }

        public async Task<BaseResponse> RemoveTourAsync(int tourId, AppUser appUser)
        {
            try
            {
                _logger.LogInformation("Attempting to delete tour with ID: {TourId}", tourId);

                // Fetch the tour to be deleted
                Tour deletingTour = await _context.Tours.FirstOrDefaultAsync(h => h.Id == tourId && h.IsDeleted == false);
                if (deletingTour == null)
                {
                    _logger.LogWarning("Tour with ID: {TourId} not found", tourId);
                    return new BaseResponse
                    {
                        Success = false,
                        Message = "Tour Could Not Found",
                        StatusCode = "404",
                        Data = new List<GetTourVM>()
                    };
                }

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Mark the tour as deleted
                        deletingTour.IsDeleted = true;
                        deletingTour.DeletedBy = $"{appUser.Name} {appUser.SurName}";

                        _logger.LogInformation("Tour with ID: {TourId} marked as deleted by {DeletedBy}", tourId, deletingTour.DeletedBy);

                        // Remove related TourByCarType entries
                        await _tourByCarTypeService.RemoveTourByCarTypeAsyncWhenTourRemoving(tourId);

                        _logger.LogInformation("Related TourByCarType entries removed for tour ID: {TourId}", tourId);

                        // Commit transaction
                        await transaction.CommitAsync();
                        _logger.LogInformation("Transaction committed for tour ID: {TourId}", tourId);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "Transaction rolled back due to an error for tour ID: {TourId}", tourId);
                        throw;
                    }
                }

                // Retrieve the updated list of tours
                List<GetTourVM> tours = await GetToursAsync(1);
                int toursCount = await _context.Tours.CountAsync(t => t.IsDeleted == false);
                int pageSize = (int)Math.Ceiling((decimal)toursCount / 6);

                _logger.LogInformation("Tour with ID: {TourId} deleted successfully. Total tours count: {ToursCount}, Page size: {PageSize}", tourId, toursCount, pageSize);

                return new BaseResponse
                {
                    Success = true,
                    Message = $"Tour {deletingTour.Name} is deleted successfully.",
                    Data = tours,
                    PageSize = pageSize,
                    PageIndex = 1
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error occurred while deleting tour with ID: {TourId}", tourId);
                return new BaseResponse
                {
                    Success = false,
                    StatusCode = "500",
                    Message = "Tour Could Not Deleted Successfully, Unhandled error occurred",
                    Data = new List<GetTourVM>()
                };
            }
        }

        public async Task<BaseResponse> AddTourAsync(AddTourVM tourVM, AppUser appUser)
        {
            try
            {
                _logger.LogInformation("Attempting to add new tour with name: {TourName}", tourVM.Name);

                if (string.IsNullOrEmpty(tourVM.Name))
                {
                    List<GetTourVM> toursInDb = await GetToursAsync(1);
                    int toursCount = await _context.Tours.CountAsync(t => t.IsDeleted == false);
                    int pageSize = (int)Math.Ceiling((decimal)toursCount / 6);

                    _logger.LogWarning("Tour name is empty. Returning error response.");

                    return new BaseResponse
                    {
                        Message = "Tour Name cannot be empty",
                        StatusCode = "200",
                        Success = false,
                        Data = toursInDb,
                        PageIndex = 1,
                        PageSize = pageSize
                    };
                }

                List<string> tourNamesInDB = await _context.Tours
                    .Where(h => h.IsDeleted == false)
                    .Select(h => h.Name)
                    .ToListAsync();

                if (tourNamesInDB.Any(hn => hn.ToLower() == tourVM.Name.Trim().ToLower()))
                {
                    List<GetTourVM> toursInDb = await GetToursAsync(1);
                    int toursCount = await _context.Tours.CountAsync(t => t.IsDeleted == false);
                    int pageSize = (int)Math.Ceiling((decimal)toursCount / 6);

                    _logger.LogWarning("Tour with name: {TourName} already exists. Returning error response.", tourVM.Name);

                    return new BaseResponse
                    {
                        Message = $"Tour {tourVM.Name} already exists",
                        StatusCode = "200",
                        Success = false,
                        Data = toursInDb,
                        PageIndex = 1,
                        PageSize = pageSize
                    };
                }

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        Tour newTour = new Tour
                        {
                            Name = tourVM.Name,
                            CreatedBy = $"{appUser.Name} {appUser.SurName}",
                            Itineraries = tourVM.Itineraries.Select(it => new Itinerary { Description = it }).ToList()
                        };

                        _logger.LogInformation("Creating new tour: {TourName} with itineraries: {Itineraries}", newTour.Name, string.Join(", ", newTour.Itineraries.Select(i => i.Description)));

                        await _context.Tours.AddAsync(newTour);
                        await _context.SaveChangesAsync();

                        var newTourInDb = await _context.Tours
                            .FirstOrDefaultAsync(ct => ct.Name == tourVM.Name.Trim() && ct.IsDeleted == false);

                        if (newTourInDb != null)
                        {
                            _logger.LogInformation("New tour added with ID: {TourId}. Creating TourByCarType entries.", newTourInDb.Id);

                            await _tourByCarTypeService.CreateTourByCarTypeAsyncWhenNewTourCreating(newTourInDb.Id);

                            _logger.LogInformation("TourByCarType entries created for tour ID: {TourId}", newTourInDb.Id);
                        }

                        await transaction.CommitAsync();

                        _logger.LogInformation("Transaction committed. Tour {TourName} created successfully.", newTour.Name);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "Transaction rolled back due to an error while adding tour: {TourName}", tourVM.Name);
                        throw;
                    }
                }

                List<GetTourVM> tours = await GetToursAsync(1);
                int toursCountInDb = await _context.Tours.CountAsync(t => t.IsDeleted == false);
                int pageSizeForTours = (int)Math.Ceiling((decimal)toursCountInDb / 6);

                _logger.LogInformation("Tour created successfully. Total tours count: {ToursCount}, Page size: {PageSizeForTours}", toursCountInDb, pageSizeForTours);

                return new BaseResponse
                {
                    Data = tours,
                    Message = "Tour Created Successfully",
                    StatusCode = "201",
                    Success = true,
                    PageIndex = 1,
                    PageSize = pageSizeForTours
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error occurred while creating tour with name: {TourName}", tourVM.Name);
                return new BaseResponse
                {
                    Message = "Tour Could Not Be Created Successfully, Unhandled error occurred",
                    StatusCode = "500",
                    Success = false,
                    Data = new List<GetTourVM>()
                };
            }
        }

        public async Task<BaseResponse> GetTourByIdAsync(int tourId)
        {
            try
            {
                _logger.LogInformation("Attempting to get tour by ID: {TourId}", tourId);

                Tour tourEntity = await _context.Tours
                    .FirstOrDefaultAsync(h => h.IsDeleted == false && h.Id == tourId);

                if (tourEntity == null)
                {
                    _logger.LogWarning("Tour with ID: {TourId} not found.", tourId);

                    return new BaseResponse
                    {
                        Message = "Tour could not be found by its ID",
                        StatusCode = "404",
                        Success = false,
                        Data = new EditTourVM()
                    };
                }

                List<string> itinerariesOfTour = await _context.Itineraries
                    .Where(i => i.TourId == tourId)
                    .Select(i => i.Description)
                    .ToListAsync();

                if (itinerariesOfTour == null || itinerariesOfTour.Count == 0)
                {
                    itinerariesOfTour = new List<string>();
                }

                EditTourVM tourForEdit = new EditTourVM
                {
                    Id = tourEntity.Id,
                    Name = tourEntity.Name,
                    Itineraries = itinerariesOfTour
                };

                _logger.LogInformation("Tour retrieved successfully: {TourId}", tourId);

                return new BaseResponse
                {
                    Success = true,
                    Data = tourForEdit,
                    StatusCode = "200"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error occurred while retrieving tour with ID: {TourId}", tourId);

                return new BaseResponse
                {
                    Success = false,
                    Data = new EditTourVM(),
                    StatusCode = "500",
                    Message = "Unhandled error occurred"
                };
            }
        }

        public async Task<BaseResponse> EditTourAsync(EditTourVM tour, AppUser appUser)
        {
            if (tour == null || tour.Id <= 0)
            {
                return new BaseResponse
                {
                    Success = false,
                    Message = "Invalid tour ID.",
                    StatusCode = "400",
                    Data = tour
                };
            }

            if (string.IsNullOrWhiteSpace(tour.Name))
            {
                return new BaseResponse
                {
                    Success = false,
                    Message = "Tour name cannot be empty.",
                    StatusCode = "400",
                    Data = tour
                };
            }

            try
            {
                bool tourExists = await _context.Tours.AnyAsync(t => t.Name.ToLower() == tour.Name.ToLower().Trim()
                                                                                && t.IsDeleted == false
                                                                                && t.Id != tour.Id);

                if (tourExists)
                {
                    return new BaseResponse
                    {
                        Message = $"Tour {tour.Name} already exists.",
                        StatusCode = "400",
                        Success = false,
                        Data = tour
                    };
                }

                Tour editTour = await _context.Tours
                    .Include(t => t.Itineraries)
                    .FirstOrDefaultAsync(c => c.Id == tour.Id && !c.IsDeleted);

                if (editTour == null)
                {
                    return new BaseResponse
                    {
                        Success = false,
                        Message = "Tour not found.",
                        StatusCode = "404",
                        Data = tour
                    };
                }

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Remove old itineraries
                        _context.Itineraries.RemoveRange(editTour.Itineraries);

                        // Update tour details
                        editTour.Name = tour.Name;
                        editTour.UpdatedBy = $"{appUser.Name} {appUser.SurName}";

                        // Add new itineraries
                        var newItineraries = tour.Itineraries.Select(itinerary => new Itinerary
                        {
                            Description = itinerary,
                            TourId = tour.Id
                        }).ToList();

                        await _context.Itineraries.AddRangeAsync(newItineraries);
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "Error occurred while updating tour with ID: {TourId}", tour.Id);
                        throw;
                    }
                }

                var updatedTour = await _context.Tours
                    .Where(t => t.Id == tour.Id && !t.IsDeleted)
                    .Select(t => new EditTourVM
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Itineraries = t.Itineraries.Select(i => i.Description).ToList()
                    }).FirstOrDefaultAsync();

                return new BaseResponse
                {
                    Data = updatedTour,
                    Message = "Tour updated successfully.",
                    Success = true,
                    StatusCode = "200"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred while editing tour with ID: {TourId}", tour.Id);

                return new BaseResponse
                {
                    Data = tour,
                    Success = false,
                    Message = "An unhandled exception occurred.",
                    StatusCode = "500"
                };
            }
        }


    }
}
