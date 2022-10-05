using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WorkBC.Admin.Models
{
    /// <summary>
    ///     Model Binder for DTParameterModel (DataTables)
    /// </summary>
    public class DataTablesModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            IQueryCollection request = bindingContext.ActionContext.HttpContext.Request.Query;
            // Retrieve request data
            var draw = Convert.ToInt32(request["draw"]);
            var start = Convert.ToInt32(request["start"]);
            var length = Convert.ToInt32(request["length"]);
            var filter = request["filter"].ToString();

            // Search
            var search = new DTSearch
            {
                Value = request["search[value]"],
                Regex = Convert.ToBoolean(request["search[regex]"])
            };
            // Order
            var o = 0;
            var order = new List<DTOrder>();
            while (!string.IsNullOrEmpty(request[$"order[{o}][column]"]))
            {
                order.Add(new DTOrder
                {
                    Column = Convert.ToInt32(request[$"order[{o}][column]"]),
                    Dir = request[$"order[{o}][dir]"]
                });
                o++;
            }

            // Columns
            var c = 0;
            var columns = new List<DTColumn>();
            while (!string.IsNullOrEmpty(request[$"columns[{c}][data]"]))
            {
                columns.Add(new DTColumn
                {
                    Data = request[$"columns[{c}][data]"],
                    Name = request[$"columns[{c}][name]"],
                    Orderable = Convert.ToBoolean(request[$"columns[{c}][orderable]"]),
                    Searchable = Convert.ToBoolean(request[$"columns[{c}][searchable]"]),
                    Search = new DTSearch
                    {
                        Value = request[$"columns[{c}][search][value]"],
                        Regex = Convert.ToBoolean(request[$"columns[{c}][search][regex]"])
                    }
                });
                c++;
            }

            var result = new DataTablesModel
            {
                Draw = draw,
                Start = start,
                Length = length,
                Search = search,
                Order = order,
                Columns = columns,
                Filter = filter
            };

            bindingContext.Result = ModelBindingResult.Success(result);

            return Task.CompletedTask;
        }
    }
}