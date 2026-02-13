using Microsoft.AspNetCore.Mvc.Filters;

namespace Vaelastrasz.Server.Filters
{
    public class GlobalEnumValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            foreach (var arg in context.ActionArguments.Values)
            {
                if (arg is null)
                    continue;

                ValidateObject(arg);
            }
        }

        private void ValidateObject(object obj)
        {
            if (obj == null) return;

            var type = obj.GetType();

            if (type.IsEnum && !Enum.IsDefined(type, obj))
                throw new ArgumentException($"Invalid enum value: {obj}");

            if (!type.IsClass && !type.IsValueType)
                return;

            foreach (var prop in type.GetProperties())
            {
                if (prop.PropertyType.IsEnum)
                {
                    var value = prop.GetValue(obj);
                    if (value != null && !Enum.IsDefined(prop.PropertyType, value))
                        throw new ArgumentException($"Invalid enum value: {value} for {prop.Name}");
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        { }
    }
}