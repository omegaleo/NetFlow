using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UnityFlow.DocumentationHelper.Example.Models;
using UnityFlow.DocumentationHelper.Library.Attributes;
using UnityFlow.DocumentationHelper.Library.Helpers;

namespace UnityFlow.DocumentationHelper.Example.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [Documentation(nameof(Index), "The index of our example", new []{ "" })]
    public async Task<IActionResult> Index()
    {
        var docs = DocumentationHelperTool.GenerateDocumentation(true).ToList();
        return View(docs);
    }
    
    [Documentation(nameof(Test), "Just a test", new []{ "" })]
    public IActionResult Test()
    {
        return Ok();
    }
}