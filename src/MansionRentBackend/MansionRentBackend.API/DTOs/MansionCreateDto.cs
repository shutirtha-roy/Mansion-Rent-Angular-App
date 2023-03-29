﻿using Autofac;
using AutoMapper;
using MansionRentBackend.Application.Services;
using VillaBO = MansionRentBackend.Application.BusinessObjects.Mansion;

namespace MansionRentBackend.API.DTOs;

public class MansionCreateDto : BaseDto
{
    public string Name { get; set; }
    public string? Details { get; set; }
    public double Rate { get; set; }
    public int Sqft { get; set; }
    public int Occupancy { get; set; }
    public string? Base64Image { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public bool IsDeleted { get; set; } = false;

    private IVillaService _villaService;
    private IMapper _mapper;

    public MansionCreateDto()
    {
    }

    public MansionCreateDto(IVillaService villaService, IMapper mapper)
    {
        _villaService = villaService;
        _mapper = mapper;
    }

    public override void ResolveDependency(ILifetimeScope scope)
    {
        base.ResolveDependency(scope);
        _villaService = _scope.Resolve<IVillaService>();
        _mapper = _scope.Resolve<IMapper>();
    }

    internal async Task CreateMantion()
    {
        var villa = _mapper.Map<VillaBO>(this);

        await _villaService.CreateVilla(villa);
    }
}