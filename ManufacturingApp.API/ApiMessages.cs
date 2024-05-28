using System.Globalization;
using System.Resources;

namespace ManufacturingApp.API
{
    public static class ApiMessages
    {
        private static readonly ResourceManager ResourceManager = new ResourceManager("ManufacturingApp.API.ApiMessages", typeof(ApiMessages).Assembly);

        private static string GetResourceString(string resourceName)
        {
            var resourceValue = ResourceManager.GetString(resourceName, CultureInfo.CurrentCulture);
            if (string.IsNullOrEmpty(resourceValue))
            {
                return $"Resource with name '{resourceName}' not found.";
            }
            return resourceValue;
        }
        public static string ErrorGettingAll => $"{GetResourceString("ErrorGettingAll")}";
        public static string ErrorGettingById => $"{GetResourceString("ErrorGettingById")}";
        public static string ErrorCreating => $"{GetResourceString("ErrorCreating")}";
        public static string ErrorUpdating => $"{GetResourceString("ErrorUpdating")}";
        public static string ErrorDeleting => $"{GetResourceString("ErrorDeleting")}";
        public static string RecipeNotFound => $"{GetResourceString("RecipeNotFound")}";
        public static string ErrorAssigningId => $"{GetResourceString("ErrorAssigningId")}";
        public static string SuccessUpdated => $"{GetResourceString("SuccessUpdated")}";
        public static string SuccessDeleted => $"{GetResourceString("SuccessDeleted")}";
        public static string ItemNotfound => $"{GetResourceString("ItemNotFound")}";

        public static string? InvalidModelWarning => $"{GetResourceString("InvalidModelWarning")}";
        public static string? NoSupplierFound => $"{GetResourceString("NoSupplierFound")}";
        public static string? ErrorOptimizeSuppliers => $"{GetResourceString("ErrorOptimizeSuppliers")}";

        public static object NoRawMaterialsFound => $"{GetResourceString("NoRawMaterialsFound")}";
    }
}
