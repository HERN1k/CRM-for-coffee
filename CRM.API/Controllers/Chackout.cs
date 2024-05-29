using CRM.API.Types;

using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers
{
  [ApiController]
  [Route("Api/[controller]")]
  public class ChackoutController : ControllerBase, IChackoutController
  {

  }
}