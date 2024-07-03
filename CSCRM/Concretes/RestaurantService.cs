using CSCRM.Abstractions;
using CSCRM.DataAccessLayers;
using CSCRM.Models.ResponseTypes;
using CSCRM.Models;
using CSCRM.ViewModels.RestaurantVMs;
using Microsoft.EntityFrameworkCore;

namespace CSCRM.Concretes
{
    public class RestaurantService : IRestaurantService
    {
        readonly AppDbContext _context;
        public RestaurantService(AppDbContext context)
        {
            _context = context;
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
                if (addRestaurantVM == null || string.IsNullOrEmpty(addRestaurantVM.Name))
                {
                    List<GetRestaurantVM> restaurantsInDb = await GetRestaurantsAsync(1);
                    int restaurantsCount = await _context.Restaurants.CountAsync(r => r.IsDeleted == false);
                    int pageSize = (int)Math.Ceiling((decimal)restaurantsCount / 6);
                    return new BaseResponse
                    {
                        Message = "Restaurant Name can not be empty",
                        StatusCode = "201",
                        Success = true,
                        Data = restaurantsInDb,
                        PageIndex = 1,
                        PageSize = pageSize
                    };
                }

                var restaurantNamesInDB = await _context.Restaurants.Where(r => r.IsDeleted == false).Select(r => r.Name).ToListAsync();
                if (restaurantNamesInDB.Any(rn => rn.ToLower() == addRestaurantVM.Name.Trim().ToLower()))
                {
                    List<GetRestaurantVM> restaurantsInDb = await GetRestaurantsAsync(1);
                    int restaurantsCount = await _context.Restaurants.CountAsync(r => r.IsDeleted == false);
                    int pageSize = (int)Math.Ceiling((decimal)restaurantsCount / 6);
                    return new BaseResponse
                    {
                        Message = $"Restaurant {addRestaurantVM.Name} is already exists",
                        StatusCode = "201",
                        Success = true,
                        Data = restaurantsInDb,
                        PageIndex = 1,
                        PageSize = pageSize
                    };
                }

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
                await _context.Restaurants.AddAsync(newRestaurant);
                await _context.SaveChangesAsync();
                List<GetRestaurantVM> restaurants = await GetRestaurantsAsync(1);
                int restaurantsCountInDb = await _context.Restaurants.CountAsync(r => r.IsDeleted == false);
                int pageSizeForRestaurants = (int)Math.Ceiling((decimal)restaurantsCountInDb / 6);

                return new BaseResponse
                {
                    Data = restaurants,
                    Message = "Restaurant Created Successfully",
                    StatusCode = "201",
                    Success = true,
                    PageIndex = 1,
                    PageSize = pageSizeForRestaurants
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Message = "Restaurant Could Not Created Successfully",
                    StatusCode = "500",
                    Success = false,
                    Data = new List<GetRestaurantVM>()
                };
            }
        }

        public async Task<BaseResponse> GetAllRestaurantsAsync(short pageIndex)
        {
            try
            {
                List<GetRestaurantVM> restaurants = await GetRestaurantsAsync(pageIndex);
                var restaurantsCount = await _context.Restaurants.CountAsync(r => r.IsDeleted == false);
                int pageSize = (int)Math.Ceiling((decimal)restaurantsCount / 6);
                return restaurants.Any()
                ? new BaseResponse { Data = restaurants, Success = true, StatusCode = "201", PageIndex = pageIndex, PageSize = pageSize }
                : new BaseResponse { Data = new List<GetRestaurantVM>(), Message = "No restaurant found", Success = true, StatusCode = "200" };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    StatusCode = "500",
                    Message = "Unhandled Error Occured",
                    Success = false,
                    Data = new List<GetRestaurantVM>()
                };
            }
        }

        public async Task<BaseResponse> RemoveRestaurantAsync(int restaurantId, AppUser appUser)
        {
            try
            {
                Restaurant deletingRestaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id == restaurantId && r.IsDeleted == false);
                if (deletingRestaurant == null)
                {
                    List<GetRestaurantVM> restaurantsInDb = await GetRestaurantsAsync(1);
                    int restaurantsCount = await _context.Restaurants.CountAsync(r => r.IsDeleted == false);
                    int pageSize = (int)Math.Ceiling((decimal)restaurantsCount / 6);
                    return new BaseResponse
                    {
                        Success = false,
                        Message = "Restaurant Could Not Found",
                        StatusCode = "404",
                        Data = restaurantsInDb,
                        PageIndex = 1,
                        PageSize = pageSize
                    };
                }

                deletingRestaurant.IsDeleted = true;
                deletingRestaurant.DeletedBy = appUser.Name + " " + appUser.SurName;
                await _context.SaveChangesAsync();
                List<GetRestaurantVM> restaurants = await GetRestaurantsAsync(1);
                int restaurantsCountInDb = await _context.Restaurants.CountAsync(r => r.IsDeleted == false);
                int pageSizeForRestaurants = (int)Math.Ceiling((decimal)restaurantsCountInDb / 6);
                return new BaseResponse
                {
                    Success = true,
                    Message = $"Restaurant {deletingRestaurant.Name} is deleted successfully.",
                    Data = restaurants,
                    StatusCode = "203",
                    PageIndex = 1,
                    PageSize = pageSizeForRestaurants
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Success = false,
                    StatusCode = "500",
                    Message = "Restaurant Could Not Deleted Successfully",
                    Data = new List<GetRestaurantVM>()
                };
            }
        }

        public async Task<BaseResponse> GetRestaurantByIdAsync(int id)
        {
            try
            {
                Restaurant restaurantEntity = await _context.Restaurants.FirstOrDefaultAsync(r => r.IsDeleted == false && r.Id == id);
                if (restaurantEntity == null)
                {
                    return new BaseResponse
                    {
                        Message = "Restaurant Could Not Found",
                        StatusCode = "404",
                        Success = false,
                        Data = new EditRestaurantVM()
                    };
                }

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
                return new BaseResponse
                {
                    Success = true,
                    Data = restaurantForEdit,
                    StatusCode = "201"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Success = false,
                    Data = new EditRestaurantVM(),
                    StatusCode = "500",
                    Message = "Unhandled error occured"
                };
            }
        }

        public async Task<BaseResponse> EditRestaurantAsync(EditRestaurantVM restaurant, AppUser appUser)
        {
            if (string.IsNullOrWhiteSpace(restaurant.Name))
            {
                return new BaseResponse
                {
                    Success = false,
                    Message = "Restaurant name cannot be empty.",
                    StatusCode = "400",
                    Data = restaurant
                };
            }
            if (restaurant == null || restaurant.Id <= 0)
            {
                return new BaseResponse
                {
                    Success = false,
                    Message = "Invalid restaurant ID.",
                    StatusCode = "400",
                    Data = restaurant
                };
            }

            try
            {
                bool restaurantExists = await _context.Restaurants.AnyAsync(r => r.Name.ToLower() == restaurant.Name.ToLower().Trim()
                                                                            && r.IsDeleted == false
                                                                            && r.Id != restaurant.Id);

                if (restaurantExists)
                {
                    return new BaseResponse
                    {
                        Message = $"Restaurant {restaurant.Name} is already exists",
                        StatusCode = "201",
                        Success = true,
                        Data = restaurant
                    };
                }

                Restaurant editRestaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id == restaurant.Id);
                if (editRestaurant == null)
                {
                    return new BaseResponse
                    {
                        Success = false,
                        Message = "Restaurant not found.",
                        StatusCode = "404",
                        Data = restaurant
                    };
                }

                string userNmSrnm = appUser.Name + " " + appUser.SurName;

                RestaurantEditor(editRestaurant, restaurant, userNmSrnm);
                await _context.SaveChangesAsync();

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

                return new BaseResponse
                {
                    Data = restaurantEdited,
                    Message = "Restaurant updated successfully.",
                    Success = true,
                    StatusCode = "200"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Data = restaurant,
                    Success = false,
                    Message = "An unhandled exception occurred.",
                    StatusCode = "500"
                };
            }
        }
    }

}
