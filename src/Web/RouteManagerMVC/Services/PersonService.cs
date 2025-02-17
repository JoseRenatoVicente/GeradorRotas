﻿using RouteManager.Domain.Core.Services;
using RouteManagerMVC.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RouteManagerMVC.Services;

public interface IPersonService
{
    Task<PersonViewModel> AddPersonAsync(PersonViewModel person);
    Task<PersonViewModel> GetPersonByIdAsync(string id);
    Task<IEnumerable<PersonViewModel>> GetPersonsByIdsAsync(IEnumerable<string> ids);
    Task<IEnumerable<PersonViewModel>> GetPersonsAsync(bool available = false);
    Task RemovePersonAsync(string id);
    Task<PersonViewModel> UpdatePersonAsync(PersonViewModel person);
}

public class PersonService : IPersonService
{
    private readonly GatewayService _gatewayService;

    public PersonService(GatewayService gatewayService)
    {
        _gatewayService = gatewayService;
    }

    public async Task<IEnumerable<PersonViewModel>> GetPersonsAsync(bool available = false)
    {
        return await _gatewayService.GetFromJsonAsync<IEnumerable<PersonViewModel>>("Teams/api/v1/People?available=" + available);
    }

    public async Task<PersonViewModel> GetPersonByIdAsync(string id)
    {
        return await _gatewayService.GetFromJsonAsync<PersonViewModel>("Teams/api/v1/People/" + id);
    }

    public async Task<IEnumerable<PersonViewModel>> GetPersonsByIdsAsync(IEnumerable<string> ids)
    {
        return await _gatewayService.GetFromJsonAsync<IEnumerable<PersonViewModel>>("Teams/api/v1/People/list/" + string.Concat(ids.Select(c => c + ",")));
    }

    public async Task<PersonViewModel> AddPersonAsync(PersonViewModel person)
    {
        return await _gatewayService.PostAsync<PersonViewModel>("Teams/api/v1/People/", person);
    }

    public async Task<PersonViewModel> UpdatePersonAsync(PersonViewModel person)
    {
        return await _gatewayService.PutAsync<PersonViewModel>("Teams/api/v1/People/" + person.Id, person);
    }

    public async Task RemovePersonAsync(string id)
    {
        await _gatewayService.DeleteAsync("Teams/api/v1/People/" + id);
    }

}