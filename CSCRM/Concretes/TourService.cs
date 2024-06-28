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
        readonly UserManager<AppUser> _userManager;
        public TourService(AppDbContext context, UserManager<AppUser> userManager)
        {
                    _context = context;
            _userManager = userManager;
        }
        private async Task<List<GetTourVM>> GetToursAsync()
        {
                                 return await _context.Tours
                                                      .Where(c => c.IsDeleted == false)
                                                      .Include(t => t.Itineraries)
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
        public async Task<BaseResponse> GetAllToursAsync()
        {
            try
            {
               List<GetTourVM> tours = await GetToursAsync();


                return tours.Any()
                ? new BaseResponse { Data = tours, Success = true, StatusCode = "200" }
                : new BaseResponse { Data = new List<GetTourVM>(), Message = "No tour found", Success = true, StatusCode = "200" };
               

            }
            catch (Exception ex)
            {
                return new BaseResponse 
                { 
                    StatusCode = "500",
                    Message = "Unhandled error occured", 
                    Success = false, 
                    Data= new List<GetTourVM>() 
                };
            }

        }
        public async Task<BaseResponse> RemoveTourAsync(int tourId, AppUser appUser)
        {
            try
            {
                Tour deletingTour = await _context.Tours.FirstOrDefaultAsync(h => h.Id == tourId && h.IsDeleted == false);
                if (deletingTour == null) 
                { 
                    return new BaseResponse 
                    { 
                        Success = false, 
                        Message = "Tour Could Not Found", 
                        StatusCode = "404", 
                        Data=new List<GetTourVM>()
                    }; 
                }


                deletingTour.IsDeleted = true;
                deletingTour.DeletedBy = appUser.Name + " " + appUser.SurName;        
                await _context.SaveChangesAsync();
                List<GetTourVM> tours = await GetToursAsync();

                return new BaseResponse 
                { 
                    Success = true, 
                    Message = $"Tour {deletingTour.Name} is deleted successfully.", 
                    Data = tours
                };
            }

            catch (Exception ex)
            {
                return new BaseResponse 
                { 
                    Success = false,
                    StatusCode = "500", 
                    Message = "Tour Could Not Deleted Successfully, Unhandled error occured", 
                    Data=new List<GetTourVM>()
                }; 
            }
        }
        public async Task<BaseResponse> AddTourAsync(AddTourVM tourVM, AppUser appUser)
        {
            try
            {
                if (string.IsNullOrEmpty(tourVM.Name))
                {
                    List<GetTourVM> toursInDb = await GetToursAsync();

                    return new BaseResponse 
                    { 
                        Message = $"Tour Name can not be empty", 
                        StatusCode = "200", 
                        Success = false,
                        Data = toursInDb
                    };

                }

                List<string> tourNamesInDB = await _context.Tours.Where(h => h.IsDeleted == false).Select(h => h.Name).ToListAsync();
                if (tourNamesInDB.Any(hn => hn.ToLower() == tourVM.Name.Trim().ToLower()))
                {
                    List<GetTourVM> toursInDb = await GetToursAsync();
                    return new BaseResponse 
                    { 
                        Message = $"Tour {tourVM.Name} is already exists", 
                        StatusCode = "200", 
                        Success = false,
                        Data = toursInDb 
                    };
                }

                Tour newTour = new Tour
                {
                    Name=tourVM.Name,
                    CreatedBy=appUser.Name + " " +appUser.SurName,
                    Itineraries=new List<Itinerary>()
                };

                foreach(var itinerary in tourVM.Itineraries)
                {
                    newTour.Itineraries.Add(new Itinerary { Description=itinerary});
                }


                await _context.Tours.AddAsync(newTour);
                await _context.SaveChangesAsync();

                List<GetTourVM> tours = await GetToursAsync();

                return new BaseResponse 
                { 
                    Data = tours, 
                    Message = "Tour Created Successfully", 
                    StatusCode = "201", 
                    Success = true 
                };
            }
            catch (Exception ex)
            {              
                return new BaseResponse 
                { 
                    Message = "Tour Could Not Created Successfully, Unhadled error occured", 
                    StatusCode = "500", 
                    Success = false, 
                    Data=new List<GetTourVM>() 
                };
            }
        }
        public async Task<BaseResponse> GetTourByIdAsync(int tourId)
        {
            try
            {
                Tour tourEntity = await _context.Tours.FirstOrDefaultAsync(h => h.IsDeleted == false && h.Id == tourId);
                if (tourEntity == null)
                {
                    return new BaseResponse 
                    { 
                        Message = "Tour Could Not Found by Its Property", 
                        StatusCode = "404", 
                        Success = false, 
                        Data = new EditTourVM() 
                    };
                }

                List<string> itinerariesOfTour = await _context.Itineraries.Where(i => i.TourId == tourId).Select(i => i.Description )
                .ToListAsync();
                if (itinerariesOfTour == null || itinerariesOfTour.Count == 0)
                {
                  itinerariesOfTour= new List<string>();
                }



                EditTourVM tourForEdit = new EditTourVM
                {
                    Id = tourEntity.Id,
                    Name = tourEntity.Name,
                    Itineraries=itinerariesOfTour,
                };
                return new BaseResponse 
                { 
                    Success = true,
                    Data = tourForEdit, 
                    StatusCode = "200" 
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse 
                { 
                    Success = false, 
                    Data = new EditTourVM(), 
                    StatusCode = "500",
                    Message = "Unhandled error occured"
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
                Tour editTour = await _context.Tours.FirstOrDefaultAsync(c => c.Id == tour.Id && !c.IsDeleted);
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
                       
                        List<Itinerary> itineraries = await _context.Itineraries.Where(i => i.TourId == tour.Id).ToListAsync();
                        _context.RemoveRange(itineraries);

                       
                        editTour.Name = tour.Name;
                        editTour.UpdatedBy = appUser.Name + " " + appUser.SurName;

                        List<Itinerary> newItineraries = tour.Itineraries.Select(itinerary => new Itinerary
                        {
                            Description = itinerary,
                            TourId = tour.Id
                        }).ToList();

                        await _context.Itineraries.AddRangeAsync(newItineraries);
                        await _context.SaveChangesAsync();

                        
                        await transaction.CommitAsync();
                    }
                    catch
                    {
                        
                        await transaction.RollbackAsync();
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
