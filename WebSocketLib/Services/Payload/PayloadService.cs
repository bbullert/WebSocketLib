using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using WebSocketLib.Models;

namespace WebSocketLib.Services
{
    public class PayloadService : IPayloadService
    {
        public WebSocketHubMethodInfo? GetMethod(string methodName, string[] parameters, Type type)
        {
            var methodInfo = GetMethodInfo(methodName, parameters, type);
            if (methodInfo == null)
                methodInfo = GetMethodInfo(methodName + "Async", parameters, type);
            if (methodInfo == null) return null;

            var typedParams = GetParameters(methodInfo, parameters);

            return new WebSocketHubMethodInfo()
            {
                MethodInfo = methodInfo,
                Parameters = typedParams
            };
        }

        private MethodInfo? GetMethodInfo(string actionName, string[] parameters, Type type)
        {
            if (string.IsNullOrEmpty(actionName)) return null;

            BindingFlags bindingFlags =
                BindingFlags.IgnoreCase |
                BindingFlags.Public |
                BindingFlags.Instance;

            var methodInfo = type.GetMethods(bindingFlags)
                .FirstOrDefault(
                    x => x.Name == actionName &&
                    x.GetParameters().Length == parameters.Length
                );

            return methodInfo;
        }

        private dynamic[] GetParameters(MethodInfo method, string[] parameters)
        {
            var parameterInfos = method.GetParameters();

            if (parameterInfos.Length != parameters.Length)
                throw new ArgumentException($"{nameof(parameters)} size must match {nameof(parameterInfos)} size");

            var typedParams = new dynamic[parameters.Count()];
            for (int i = 0; i < typedParams.Length; i++)
            {
                Type type = parameterInfos[i].ParameterType;
                var converter = TypeDescriptor.GetConverter(type);

                if (converter != null && converter.IsValid(parameters[i]))
                {
                    var value = converter.ConvertFromString(null, CultureInfo.InvariantCulture, parameters[i]);
                    typedParams[i] = value;
                }
            }
            return typedParams;
        }
    }
}
