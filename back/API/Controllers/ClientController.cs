using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using AutoMapper;
using Domine.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
    public class ClientController: ApiBaseController
    {
        private readonly IUnitOfWork unitOfWork;
    private readonly IMapper _mapper;

    public ClientController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        this.unitOfWork = unitOfWork;
        this._mapper = mapper;
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<ClientDto>>> Get()
    {
        var clients = await this.unitOfWork.Clients.GetAllAsync();
        return this._mapper.Map<List<ClientDto>>(clients);
    }

    }
