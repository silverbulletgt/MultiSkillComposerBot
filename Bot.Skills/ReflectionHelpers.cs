using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Bot.Skills
{
    public class ReflectionHelpers
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseType"></param>
        /// <param name="assemblyNames">List of names of Assembly Names to search through.  The name expected is the main project name, not the full name of the assembly with version details</param>
        /// <param name="namespaceName"></param>
        /// <param name="excludedTypes"></param>
        /// <returns></returns>
        public static List<Type> GetNonAbstractImplementationsOfType(Type baseType, List<string> assemblyNames, string namespaceName, List<Type> excludedTypes = null)
        {
            if (excludedTypes == null)
            {
                excludedTypes = new List<Type>();
            }
            var asmNames = DependencyContext.Default.GetDefaultAssemblyNames();

            List<Assembly> assembly = new List<Assembly>();

            //filter only for the assemblies we are interested in
            foreach (string assemblyName in assemblyNames)
            {
                var fullName = asmNames.Where(x => x.Name.Equals(assemblyName)).Single().FullName;

                assembly.Add(Assembly.Load(fullName));
            }

            return assembly
                .SelectMany(t => t.GetTypes().Where(t => !t.IsAbstract && !String.IsNullOrEmpty(t.Namespace) && t.Namespace.StartsWith(namespaceName)))
                .Where(p => p.GetTypeInfo().IsSubclassOf(baseType))
                .Except(excludedTypes)
                .ToList();
        }
    }
}
