﻿using Modern.Repositories.Abstractions;
using Modern.Services.DataStore.Cached.Examples.Entities;

namespace Modern.Services.DataStore.Cached.Examples.Repositories;

public interface ICityRepository : IModernRepository<CityDbo, int>
{
    Task<List<CityDbo>> GetCountryCitiesAsync(string country);
}