﻿namespace AppLanches.Services
{
    public class ApiResponse<T>
    {
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }
        public bool HasError => !string.IsNullOrEmpty(this.ErrorMessage);
    }
}