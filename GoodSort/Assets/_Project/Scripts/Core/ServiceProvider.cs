using System;
using System.Collections.Generic;
using UnityEngine; // Cần cho Debug.LogError

namespace Goodsort.Core
{
    /// <summary>
    /// Service Locator tĩnh đơn giản để quản lý và truy cập các Manager Systems.
    /// Cung cấp một cách để giảm coupling giữa các hệ thống.
    /// </summary>
    public static class ServiceProvider
    {
        private static readonly Dictionary<Type, object> services = new Dictionary<Type, object>();

        /// <summary>
        /// Đăng ký một instance của service/manager.
        /// </summary>
        /// <typeparam name="T">Kiểu của service (thường là Interface hoặc Class).</typeparam>
        /// <param name="serviceInstance">Instance của service cần đăng ký.</param>
        public static void RegisterService<T>(T serviceInstance) where T : class
        {
            Type type = typeof(T);
            if (services.ContainsKey(type))
            {
                Debug.LogError($"[ServiceProvider] Service type '{type.Name}' đã được đăng ký.");
                return;
            }
            services[type] = serviceInstance;
            // Debug.Log($"[ServiceProvider] Registered service: {type.Name}"); // Optional log
        }

        /// <summary>
        /// Lấy một instance của service đã đăng ký.
        /// </summary>
        /// <typeparam name="T">Kiểu của service cần lấy.</typeparam>
        /// <returns>Instance của service.</returns>
        /// <exception cref="Exception">Ném lỗi nếu service chưa được đăng ký.</exception>
        public static T GetService<T>() where T : class
        {
            Type type = typeof(T);
            if (!services.TryGetValue(type, out object serviceInstance))
            {
                throw new Exception($"[ServiceProvider] Service type '{type.Name}' chưa được đăng ký.");
            }
            return (T)serviceInstance;
        }

        /// <summary>
        /// (Optional) Hủy đăng ký một service.
        /// </summary>
        public static void UnregisterService<T>() where T : class
        {
            Type type = typeof(T);
            if (services.ContainsKey(type))
            {
                services.Remove(type);
                // Debug.Log($"[ServiceProvider] Unregistered service: {type.Name}"); // Optional log
            }
        }

        /// <summary>
        /// (Optional) Clear tất cả các service đã đăng ký (ví dụ khi thoát game hoặc reset).
        /// </summary>
        public static void ClearServices()
        {
            services.Clear();
            // Debug.Log("[ServiceProvider] Cleared all services."); // Optional log
        }
    }
}