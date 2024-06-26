﻿using Dinners.Application.Common;
using Dinners.Application.Restaurants.Post.Requests;
using ErrorOr;

namespace Dinners.Application.Restaurants.Post;

public sealed record PostRestaurantCommand(RestaurantInformationRequest RestaurantInformation,
    RestaurantLocalizationRequest RestaurantLocalization,
    List<RestaurantScheduleRequest> RestaurantSchedules,
    List<RestaurantTableRequest> RestaurantTables,
    List<RestaurantAdministrationRequest> RestaurantAdministrations,
    RestaurantContactRequest RestaurantContact,
    List<string> Chefs,
    List<string> Specialties) : ICommand<ErrorOr<Guid>>;
