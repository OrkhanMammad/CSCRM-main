using CSCRM.Abstractions;
using CSCRM.DataAccessLayers;
using CSCRM.Models;
using CSCRM.Models.ResponseTypes;
using CSCRM.ViewModels.CompanyVMs;
using CSCRM.ViewModels.HotelVMs;
using CSCRM.ViewModels.ItineraryVMS;
using CSCRM.ViewModels.TourVMs;
using Microsoft.EntityFrameworkCore;

namespace CSCRM.Concretes
{
    public class TourService : ITourService
    {
        readonly AppDbContext _context;
        public TourService(AppDbContext context)
        {
                    _context = context;
        }
        public async Task<BaseResponse> GetAllToursAsync()
        {
            try
            {
               List<GetTourVM> tours = await _context.Tours
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


                if (tours.Count() == 0)
                {
                    return new BaseResponse { Data = new List<GetTourVM>(), Message = "No tour found", Success = true, StatusCode = "200" };
                }
                else
                {
                    return new BaseResponse { Data = tours, Success = true, StatusCode = "201" };

                }

            }
            catch (Exception ex)
            {
                return new BaseResponse { StatusCode = "404", Message = "Unhandled error occured", Success = false, Data= new List<GetTourVM>() };
            }

        }
        public async Task<BaseResponse> RemoveTourAsync(int tourId)
        {
            try
            {
                Tour deletingTour = await _context.Tours.FirstOrDefaultAsync(h => h.Id == tourId && h.IsDeleted == false);
                if (deletingTour == null) { return new BaseResponse { Success = false, Message = "Tour Could Not Found", StatusCode = "404" }; }
                
                deletingTour.IsDeleted = true;
                await _context.SaveChangesAsync();
                List<GetTourVM> tours = await _context.Tours
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

                return new BaseResponse { Success = true, Message = $"Tour {deletingTour.Name} is deleted successfully.", Data = tours };
            }

            catch (Exception ex)
            {
                List<GetTourVM> tours = await _context.Tours
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

                return new BaseResponse { Success = false, StatusCode = "500", Message = "Tour Could Not Deleted Successfully", Data=tours }; 
            }



        }

        public async Task<BaseResponse> AddTourAsync(AddTourVM tourVM)
        {
            try
            {
                if (string.IsNullOrEmpty(tourVM.Name))
                {
                    List<GetTourVM> toursInDb = await _context.Tours
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
                    return new BaseResponse { Message = $"Tour Name can not be empty", StatusCode = "201", Success = false, Data = toursInDb };

                }

                List<string> tourNamesInDB = await _context.Tours.Where(h => h.IsDeleted == false).Select(h => h.Name).ToListAsync();
                if (tourNamesInDB.Any(hn => hn.ToLower() == tourVM.Name.Trim().ToLower()))
                {
                    List<GetTourVM> toursInDb = await _context.Tours
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
                    return new BaseResponse { Message = $"Tour {tourVM.Name} is already exists", StatusCode = "201", Success = false, Data = toursInDb };
                }

                Tour newTour = new Tour
                {
                    Name=tourVM.Name,
                    Itineraries=new List<Itinerary>()
                };

                foreach(var itinerary in tourVM.Itineraries)
                {
                    newTour.Itineraries.Add(new Itinerary { Description=itinerary});
                }


                await _context.Tours.AddAsync(newTour);
                await _context.SaveChangesAsync();
                List<GetTourVM> tours = await _context.Tours
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
                return new BaseResponse { Data = tours, Message = "Tour Created Successfully", StatusCode = "201", Success = true };


            }
            catch (Exception ex)
            {
                List<GetTourVM> tours = await _context.Tours
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
                return new BaseResponse { Message = "Tour Could Not Created Successfully, Unhadled error occured", StatusCode = "500", Success = false, Data=tours };

            }
        }

    }
}
