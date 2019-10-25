using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryLeakWithDynamic
{
    class Program
    {
        static void Main(string[] args)
        {
            // Run this program in DEBUG mode and watch the Process Memory chart in Diagnostics Tools.
            while (true)
            {
                // Enable any of the 4 lines beneath (one at a time)
                //Experiment_WithoutMemoryLeak_1();
                //Experiment_WithoutMemoryLeak_2();
                //Experiment_WithoutMemoryLeak_3();
                Experiment_WithMemoryLeak_1();

                // Forcing GC for every iteration to control GC for this experiment
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        static void Experiment_WithoutMemoryLeak_1()
        {
            var json = GenerateRandomJson();
            ExecuteWithCastingToObjectBeforeAccessingOfChildProperty(json);
        }

        static void Experiment_WithoutMemoryLeak_2()
        {
            var json = GenerateRandomJson_WithExtraProperty();
            ExecuteWithCastingToObjectBeforeAccessingOfChildProperty(json);

        }

        static void Experiment_WithoutMemoryLeak_3()
        {
            var json = GenerateRandomJson();
            ExecuteWithDynamicAccessingOfChildProperty(json);

        }

        static void Experiment_WithMemoryLeak_1()
        {
            var json = GenerateRandomJson_WithExtraProperty();
            ExecuteWithDynamicAccessingOfChildProperty(json);
        }






        static void ExecuteWithCastingToObjectBeforeAccessingOfChildProperty(string json)
        {
            var converter = new ExpandoObjectConverter();
            dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(json, converter) as IDictionary<string, object>;

            var dataAsDictionary = data as IDictionary<string, object>;
            var resultAsDictionary = dataAsDictionary["result"] as IDictionary<string, object>;
            var propertiesAsDictionary = resultAsDictionary["properties"] as IEnumerable<object>;

            foreach (var item in propertiesAsDictionary)
            {
            }
        }
        
        static void ExecuteWithDynamicAccessingOfChildProperty(string json)
        {
            var converter = new ExpandoObjectConverter();
            dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(json, converter) as IDictionary<string, object>;

            foreach (var item in data.result.properties)
            {
            }
        }


        static string GenerateRandomJson_WithExtraProperty()
        {
            return @"
            {
                ""someOtherProperty"": null,
                ""result"": {
                    ""properties"": [
                        {
                            ""id"": """ + Guid.NewGuid().ToString() + @"""
                        }
                    ]
                }
            }";
        }

        static string GenerateRandomJson()
        {
            return @"
            {
                ""result"": {
                    ""properties"": [
                        {
                            ""id"": """ + Guid.NewGuid().ToString() + @"""
                        }
                    ]
                }
            }";
        }
    }
}
