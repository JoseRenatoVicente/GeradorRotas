﻿using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RouteManager.WebAPI.Core.Notifications;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RouteManagerMVC.Controllers.Base;

//[SecurityHeaders]
public class MvcBaseController : Controller
{
    private readonly ICollection<string> _errors = new List<string>();
    private readonly INotifier _notifier;

    public MvcBaseController(INotifier notifier)
    {
        _notifier = notifier;
    }

    protected async Task<IActionResult> CustomResponseAsync(object result = null, string actionName = "Index")
    {
        if (await IsOperationValid())
        {
            return RedirectToAction(actionName);
        }
        TempData["Errors"] = await GetErrors();

        return View(result);
    }

    protected async Task<IActionResult> CustomResponseAsync(ModelStateDictionary modelState)
    {
        var erros = modelState.Values.SelectMany(e => e.Errors);
        foreach (var erro in erros)
        {
            await AddError(erro.ErrorMessage);
        }

        return await CustomResponseAsync();
    }

    protected async Task<IActionResult> CustomResponseAsync(ValidationResult validationResult)
    {
        foreach (var error in validationResult.Errors)
        {
            await AddError(error.ErrorMessage);
        }

        return await CustomResponseAsync();
    }

    protected Task<bool> IsOperationValid()
    {
        return Task.Run(() => !_notifier.IsNotified());
    }

    protected async Task<IEnumerable<string>> GetErrors()
    {
        foreach (var item in _notifier.GetNotifications())
        {
            await AddError(item);
        }
        _notifier.Clear();
        return _errors;
    }

    protected Task Notification(string error)
    {
        return Task.Run(() => _notifier.Handle(error));
    }
    private Task AddError(string error)
    {
        return Task.Run(() => _errors.Add(error));
    }

}