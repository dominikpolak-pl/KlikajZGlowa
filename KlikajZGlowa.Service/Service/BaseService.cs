using AutoMapper;
using KlikajZGlowa.Database;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Text;

namespace KlikajZGlowa.Service.Service
{
    public abstract class BaseService
    {
        protected readonly Db _dbContext;
        protected readonly IMapper _mapper;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public BaseService(Db dbContext, IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        protected string? GetCurrentUserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("sub");

        protected bool IsAuthenticated =>
            _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}
