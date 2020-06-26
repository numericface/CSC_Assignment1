using WebAPI2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Cors;

namespace WebAPI2.Controllers
{
    public class ProductsV1Controller : ApiController
    {
        //[EnableCors(origins: "http://localhost:49345,http://csc123client.azurewebsites.net", headers: "*", methods: "*")]
        public class ProductsController : ApiController
        {

            static readonly IProductRepository repository = new ProductRepository();

            [HttpGet]
            [Route("api/v3/products/{id:int:min(2)}", Name = "getProductByIdv1")]
            public Product retrieveProductfromRepository(int id)
            {
                Product item = repository.Get(id);
                if (item == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }
                return item;
            }

            [HttpGet]
            [Route("api/v1/products")]
            public IEnumerable<Product> GetAllProducts()
            {
                return repository.GetAll();
            }

            [HttpGet]
            [Route("api/v1/products/{id:int:min(1)}")]
            public Product GetProduct(int id)
            {
                Product item = repository.Get(id);
                if (item == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }
                return item;
            }


            public IEnumerable<Product> GetProductsByCategory(string category)
            {
                return repository.GetAll().Where(
                    p => string.Equals(p.Category, category, StringComparison.OrdinalIgnoreCase));
            }

            [HttpPost]
            [Route("api/v1/products")]
            public HttpResponseMessage PostProduct(Product item)
            {
                if (ModelState.IsValid)
                {
                    item = repository.Add(item);
                    var response = Request.CreateResponse<Product>(HttpStatusCode.Created, item);

                    string uri = Url.Link("getProductByIdv1", new { id = item.Id });
                    response.Headers.Location = new Uri(uri);
                    return response;
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
            }

            [HttpPut]
            [Route("api/v1/products/{id:int}")]
            public void PutProduct(int id, Product product)
            {
                product.Id = id;
                if (!repository.Update(product))
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }
            }

            [HttpDelete]
            [Route("api/v1/products/{id:int}")]
            public void DeleteProduct(int id)
            {
                Product item = repository.Get(id);
                if (item == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }

                repository.Remove(id);
            }

            [HttpGet]
            [Route("api/v1/products/version")]
            //http://localhost:9000/api/v1/products/version
            public string[] GetVersion()
            {
                return new string[]
                  {"hello",
                  "version 1",
                  "1"
                  };
            }


            [HttpGet]
            [Route("api/v1/products/message/")]
            //http://localhost:9000/api/v1/products/message?name1=ji&name2=jii1&name3=ji3
            public HttpResponseMessage GetMultipleNames(String name1, string name2, string name3)
            {
                var response = new HttpResponseMessage();
                response.Content = new StringContent("<html><body>Hello World " +
                        " name1=" + name1 +
                        " name2= " + name2 +
                        " name3=" + name3 +
                        " is provided in path parameter</body></html>");
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                return response;
            }

        }

    }
}
