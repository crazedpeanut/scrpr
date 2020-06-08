// using System.Threading.Tasks;
// using GraphQL;
// using GraphQL.Instrumentation;
// using GraphQL.Types;
// using GraphQL.Validation.Complexity;
// using Microsoft.AspNetCore.Mvc;
// using Scraper.Api.Models.GraphQL;

// namespace Scraper.Api.Controllers
// {
//     [ApiController]
//     [Route("[controller]")]
//     public class GraphQLController : ControllerBase
//     {
//         private readonly ISchema schema;
//         private readonly IDocumentExecuter executer;

//         public GraphQLController(
//             IDocumentExecuter executer,
//             ISchema schema)
//         {
//             this.executer = executer;
//             this.schema = schema;
//         }

//         public async Task<IActionResult> Post([FromBody] GraphQLQuery query)
//         {
//             var inputs = query.Variables.ToInputs();

//             var result = await executer.ExecuteAsync(_ =>
//             {
//                 _.Schema = schema;
//                 _.Query = query.Query;
//                 _.OperationName = query.OperationName;
//                 _.Inputs = inputs;

//                 _.ComplexityConfiguration = new ComplexityConfiguration { MaxDepth = 15 };
//                 _.FieldMiddleware.Use<InstrumentFieldsMiddleware>();
//             });

//             if (result.Errors?.Count > 0)
//             {
//                 return BadRequest();
//             }

//             return Ok(result);
//         }
//     }
// }