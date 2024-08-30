using CSCRM.Abstractions;
using CSCRM.dataAccessLayers;
using CSCRM.Models.ResponseTypes;
using CSCRM.Models;
using CSCRM.ViewModels.RestaurantVMs;
using Microsoft.EntityFrameworkCore;

namespace CSCRM.Concretes
{
    public class RestaurantService : IRestaurantService
    {
        readonly AppDbContext _context;
        private readonly ILogger<RestaurantService> _logger;
        public RestaurantService(AppDbContext context, ILogger<RestaurantService> logger)
        {
            _context = context;
            _logger = logger;
        }

        private void RestaurantEditor(Restaurant restaurant, EditRestaurantVM updatedRestaurant, string userNmSrnm)
        {
            restaurant.Name = updatedRestaurant.Name.Trim();
            restaurant.ContactPerson = updatedRestaurant.ContactPerson.Trim();
            restaurant.ContactPhone = updatedRestaurant.ContactPhone.Trim();
            restaurant.Lunch = updatedRestaurant.Lunch;
            restaurant.Dinner = updatedRestaurant.Dinner;
            restaurant.Gala_Dinner_Simple = updatedRestaurant.Gala_Dinner_Simple;
            restaurant.Gala_Dinner_Local_Alc = updatedRestaurant.Gala_Dinner_Local_Alc;
            restaurant.Gala_Dinner_Foreign_Alc = updatedRestaurant.Gala_Dinner_Foreign_Alc;
            restaurant.TakeAway = updatedRestaurant.TakeAway;
            restaurant.UpdatedBy = userNmSrnm;
        }

        private async Task<List<GetRestaurantVM>> GetRestaurantsAsync(short pageIndex)
        {
            return await _context.Restaurants
                                 .Where(r => r.IsDeleted == false)
                                 .OrderByDescending(r => r.Id)
                                 .Skip((pageIndex - 1) * 6)
                                 .Take(6)
                                 .Select(r => new GetRestaurantVM
                                 {
                                     Id = r.Id,
                                     Name = r.Name,
                                     ContactPerson = r.ContactPerson,
                                     ContactPhone = r.ContactPhone,
                                     Lunch = r.Lunch,
                                     Dinner = r.Dinner,
                                     Gala_Dinner_Simple = r.Gala_Dinner_Simple,
                                     Gala_Dinner_Local_Alc = r.Gala_Dinner_Local_Alc,
                                     Gala_Dinner_Foreign_Alc = r.Gala_Dinner_Foreign_Alc,
                                     TakeAway = r.TakeAway
                                 })
                                 .ToListAsync();
        }

        public async Task<BaseResponse> AddRestaurantAsync(AddRestaurantVM addRestaurantVM, AppUser appUser)
        {
            try
            {
                // Log request details
                _logger.LogInformation("Attempting to add a new restaurant. Request data: {@AddRestaurantVM}", addRestaurantVM);

                // Check if the input is valid
                if (addRestaurantVM == null || string.IsNullOrEmpty(addRestaurantVM.Name))
                {
                    _logger.LogWarning("Restaurant name cannot be empty. Request data: {@AddRestaurantVM}", addRestaurantVM);

                    List<GetRestaurantVM> restaurantsInDb = await GetRestaurantsAsync(1);
                    int restaurantsCount = await _context.Restaurants.CountAsync(r => r.IsDeleted == false);
                    int pageSize = (int)Math.Ceiling((decimal)restaurantsCount / 6);

                    return new BaseResponse
                    {
                        Message = "Restaurant Name cannot be empty",
                        StatusCode = "201",
                        Success = true,
                        data = restaurantsInDb,
                        PageIndex = 1,
                        PageSize = pageSize
                    };
                }

                // Check if a restaurant with the same name already exists
                var restaurantNamesInDB = await _context.Restaurants
                    .Where(r => r.IsDeleted == false)
                    .Select(r => r.Name)
                    .ToListAsync();

                if (restaurantNamesInDB.Any(rn => rn.ToLower() == addRestaurantVM.Name.Trim().ToLower()))
                {
                    _logger.LogWarning("Restaurant with name {RestaurantName} already exists. Request data: {@AddRestaurantVM}", addRestaurantVM.Name, addRestaurantVM);

                    List<GetRestaurantVM> restaurantsInDb = await GetRestaurantsAsync(1);
                    int restaurantsCount = await _context.Restaurants.CountAsync(r => r.IsDeleted == false);
                    int pageSize = (int)Math.Ceiling((decimal)restaurantsCount / 6);

                    return new BaseResponse
                    {
                        Message = $"Restaurant {addRestaurantVM.Name} already exists",
                        StatusCode = "201",
                        Success = true,
                        data = restaurantsInDb,
                        PageIndex = 1,
                        PageSize = pageSize
                    };
                }

                // Create new restaurant
                Restaurant newRestaurant = new Restaurant
                {
                    Name = addRestaurantVM.Name.Trim(),
                    ContactPerson = addRestaurantVM.ContactPerson.Trim(),
                    ContactPhone = addRestaurantVM.ContactPhone.Trim(),
                    Lunch = addRestaurantVM.Lunch,
                    Dinner = addRestaurantVM.Dinner,
                    Gala_Dinner_Simple = addRestaurantVM.Gala_Dinner_Simple,
                    Gala_Dinner_Local_Alc = addRestaurantVM.Gala_Dinner_Local_Alc,
                    Gala_Dinner_Foreign_Alc = addRestaurantVM.Gala_Dinner_Foreign_Alc,
                    TakeAway = addRestaurantVM.TakeAway,
                    CreatedBy = appUser.Name + " " + appUser.SurName
                };

                _logger.LogInformation("Adding new restaurant: {@NewRestaurant}", newRestaurant);

                await _context.Restaurants.AddAsync(newRestaurant);
                await _context.SaveChangesAsync();

                List<GetRestaurantVM> restaurants = await GetRestaurantsAsync(1);
                int restaurantsCountInDb = await _context.Restaurants.CountAsync(r => r.IsDeleted == false);
                int pageSizeForRestaurants = (int)Math.Ceiling((decimal)restaurantsCountInDb / 6);

                _logger.LogInformation("Restaurant created successfully. Restaurant data: {@NewRestaurant}", newRestaurant);

                return new BaseResponse
                {
                    data = restaurants,
                    Message = "Restaurant Created Successfully",
                    StatusCode = "201",
                    Success = true,
                    PageIndex = 1,
                    PageSize = pageSizeForRestaurants
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a restaurant. Request data: {@AddRestaurantVM}", addRestaurantVM);
                return new BaseResponse
                {
                    Message = "Restaurant Could Not Be Created Successfully",
                    StatusCode = "500",
                    Success = false,
                    data = new List<GetRestaurantVM>()
                };
            }
        }


        public async Task<BaseResponse> GetAllRestaurantsAsync(short pageIndex)
        {
            try
            {
                _logger.LogInformation("Fetching all restaurants. PageIndex: {PageIndex}", pageIndex);

                List<GetRestaurantVM> restaurants = await GetRestaurantsAsync(pageIndex);
                var restaurantsCount = await _context.Restaurants.CountAsync(r => r.IsDeleted == false);
                int pageSize = (int)Math.Ceiling((decimal)restaurantsCount / 6);

                if (restaurants.Any())
                {
                    _logger.LogInformation("Successfully fetched restaurants. PageIndex: {PageIndex}, TotalRestaurants: {TotalRestaurants}", pageIndex, restaurantsCount);
                    return new BaseResponse
                    {
                        data = restaurants,
                        Success = true,
                        StatusCode = "201",
                        PageIndex = pageIndex,
                        PageSize = pageSize
                    };
                }
                else
                {
                    _logger.LogWarning("No restaurants found. PageIndex: {PageIndex}", pageIndex);
                    return new BaseResponse
                    {
                        data = new List<GetRestaurantVM>(),
                        Message = "No restaurant found",
                        Success = true,
                        StatusCode = "200"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching restaurants. PageIndex: {PageIndex}", pageIndex);
                return new BaseResponse
                {
                    StatusCode = "500",
                    Message = "Unhandled Error Occurred",
                    Success = false,
                    data = new List<GetRestaurantVM>()
                };
            }
        }


        public async Task<BaseResponse> RemoveRestaurantAsync(int restaurantId, AppUser appUser)
        {
            try
            {
                _logger.LogInformation("Attempting to delete restaurant with ID: {RestaurantId}. User: {UserName}", restaurantId, appUser.Name + " " + appUser.SurName);

                Restaurant deletingRestaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id == restaurantId && r.IsDeleted == false);
                if (deletingRestaurant == null)
                {
                    _logger.LogWarning("Restaurant with ID: {RestaurantId} not found. Returning data.", restaurantId);

                    List<GetRestaurantVM> restaurantsInDb = await GetRestaurantsAsync(1);
                    int restaurantsCount = await _context.Restaurants.CountAsync(r => r.IsDeleted == false);
                    int pageSize = (int)Math.Ceiling((decimal)restaurantsCount / 6);

                    return new BaseResponse
                    {
                        Success = false,
                        Message = "Restaurant Could Not Found",
                        StatusCode = "404",
                        data = restaurantsInDb,
                        PageIndex = 1,
                        PageSize = pageSize
                    };
                }

                deletingRestaurant.IsDeleted = true;
                deletingRestaurant.DeletedBy = appUser.Name + " " + appUser.SurName;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted restaurant with ID: {RestaurantId}. User: {UserName}", restaurantId, appUser.Name + " " + appUser.SurName);

                List<GetRestaurantVM> restaurants = await GetRestaurantsAsync(1);
                int restaurantsCountInDb = await _context.Restaurants.CountAsync(r => r.IsDeleted == false);
                int pageSizeForRestaurants = (int)Math.Ceiling((decimal)restaurantsCountInDb / 6);

                return new BaseResponse
                {
                    Success = true,
                    Message = $"Restaurant {deletingRestaurant.Name} is deleted successfully.",
                    data = restaurants,
                    StatusCode = "203",
                    PageIndex = 1,
                    PageSize = pageSizeForRestaurants
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting restaurant with ID: {RestaurantId}", restaurantId);

                return new BaseResponse
                {
                    Success = false,
                    StatusCode = "500",
                    Message = "Restaurant Could Not Deleted Successfully",
                    data = new List<GetRestaurantVM>()
                };
            }
        }


        public async Task<BaseResponse> GetRestaurantByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Fetching restaurant with ID: {RestaurantId}", id);

                Restaurant restaurantEntity = await _context.Restaurants.FirstOrDefaultAsync(r => r.IsDeleted == false && r.Id == id);
                if (restaurantEntity == null)
                {
                    _logger.LogWarning("Restaurant with ID: {RestaurantId} not found", id);

                    return new BaseResponse
                    {
                        Message = "Restaurant Could Not Found",
                        StatusCode = "404",
                        Success = false,
                        data = new EditRestaurantVM()
                    };
                }

                _logger.LogInformation("Restaurant with ID: {RestaurantId} found. Preparing data for edit", id);

                EditRestaurantVM restaurantForEdit = new EditRestaurantVM
                {
                    Id = restaurantEntity.Id,
                    Name = restaurantEntity.Name,
                    ContactPerson = restaurantEntity.ContactPerson,
                    ContactPhone = restaurantEntity.ContactPhone,
                    Lunch = restaurantEntity.Lunch,
                    Dinner = restaurantEntity.Dinner,
                    Gala_Dinner_Simple = restaurantEntity.Gala_Dinner_Simple,
                    Gala_Dinner_Local_Alc = restaurantEntity.Gala_Dinner_Local_Alc,
                    Gala_Dinner_Foreign_Alc = restaurantEntity.Gala_Dinner_Foreign_Alc,
                    TakeAway = restaurantEntity.TakeAway
                };

                _logger.LogInformation("Restaurant with ID: {RestaurantId} data prepared successfully", id);

                return new BaseResponse
                {
                    Success = true,
                    data = restaurantForEdit,
                    StatusCode = "201"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching restaurant with ID: {RestaurantId}", id);

                return new BaseResponse
                {
                    Success = false,
                    data = new EditRestaurantVM(),
                    StatusCode = "500",
                    Message = "Unhandled error occured"
                };
            }
        }


        public async Task<BaseResponse> EditRestaurantAsync(EditRestaurantVM restaurant, AppUser appUser)
        {
            if (string.IsNullOrWhiteSpace(restaurant.Name))
            {
                _logger.LogWarning("Attempted to edit a restaurant with an empty name.");

                return new BaseResponse
                {
                    Success = false,
                    Message = "Restaurant name cannot be empty.",
                    StatusCode = "400",
                    data = restaurant
                };
            }

            if (restaurant == null || restaurant.Id <= 0)
            {
                _logger.LogWarning("Attempted to edit a restaurant with invalid ID: {RestaurantId}", restaurant?.Id);

                return new BaseResponse
                {
                    Success = false,
                    Message = "Invalid restaurant ID.",
                    StatusCode = "400",
                    data = restaurant
                };
            }

            try
            {
                _logger.LogInformation("Checking if restaurant with name: {RestaurantName} already exists.", restaurant.Name);

                bool restaurantExists = await _context.Restaurants.AnyAsync(r => r.Name.ToLower() == restaurant.Name.ToLower().Trim()
                                                                             && r.IsDeleted == false
                                                                             && r.Id != restaurant.Id);

                if (restaurantExists)
                {
                    _logger.LogWarning("Restaurant with name: {RestaurantName} already exists.", restaurant.Name);

                    return new BaseResponse
                    {
                        Message = $"Restaurant {restaurant.Name} already exists",
                        StatusCode = "201",
                        Success = true,
                        data = restaurant
                    };
                }

                _logger.LogInformation("Fetching restaurant with ID: {RestaurantId} for editing.", restaurant.Id);

                Restaurant editRestaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id == restaurant.Id);
                if (editRestaurant == null)
                {
                    _logger.LogWarning("Restaurant with ID: {RestaurantId} not found for editing.", restaurant.Id);

                    return new BaseResponse
                    {
                        Success = false,
                        Message = "Restaurant not found.",
                        StatusCode = "404",
                        data = restaurant
                    };
                }

                string userNmSrnm = appUser.Name + " " + appUser.SurName;
                _logger.LogInformation("Updating restaurant with ID: {RestaurantId} by user: {UserName}", restaurant.Id, userNmSrnm);

                RestaurantEditor(editRestaurant, restaurant, userNmSrnm);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Fetching updated restaurant with ID: {RestaurantId}", restaurant.Id);

                Restaurant restaurantEntity = await _context.Restaurants.FirstOrDefaultAsync(r => r.IsDeleted == false && r.Id == editRestaurant.Id);

                EditRestaurantVM restaurantEdited = new EditRestaurantVM
                {
                    Id = restaurantEntity.Id,
                    Name = restaurantEntity.Name,
                    ContactPerson = restaurantEntity.ContactPerson,
                    ContactPhone = restaurantEntity.ContactPhone,
                    Lunch = restaurantEntity.Lunch,
                    Dinner = restaurantEntity.Dinner,
                    Gala_Dinner_Simple = restaurantEntity.Gala_Dinner_Simple,
                    Gala_Dinner_Local_Alc = restaurantEntity.Gala_Dinner_Local_Alc,
                    Gala_Dinner_Foreign_Alc = restaurantEntity.Gala_Dinner_Foreign_Alc,
                    TakeAway = restaurantEntity.TakeAway
                };

                _logger.LogInformation("Restaurant with ID: {RestaurantId} updated successfully.", restaurant.Id);

                return new BaseResponse
                {
                    data = restaurantEdited,
                    Message = "Restaurant updated successfully.",
                    Success = true,
                    StatusCode = "200"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred while updating restaurant with ID: {RestaurantId}", restaurant.Id);

                return new BaseResponse
                {
                    data = restaurant,
                    Success = false,
                    Message = "An unhandled exception occurred.",
                    StatusCode = "500"
                };
            }
        }

    }

}
