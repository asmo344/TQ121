using System;
using System.Collections.Generic;

namespace Tyrafos.UniversalSerialBus
{
    public static class UniversalSerialBusExtension
    {
        public static UsbAttribute[] FindDifferent(this UsbAttribute[] original, UsbAttribute[] compare)
        {
            var attributes = new List<UsbAttribute>();
            var more = original.Length > compare.Length ? original : compare;
            var less = original.Length > compare.Length ? compare : original;
            foreach (var item in more)
            {
                if (!Array.Exists(less, x => x.Equals(item)))
                {
                    attributes.Add(item);
                }
            }
            return attributes.ToArray();
        }

        /// <summary>
        /// 根據index在全部的USB裝置中的位置尋找所有同樣USB ID的對應位置。
        /// 如果UsbAttribute[]長度等於0且originalIndex也等於0則返回-1。
        /// </summary>
        /// <param name="infos"></param>
        /// <param name="originalIndex"></param>
        /// <returns></returns>
        public static int FindMappingIndex(this UsbAttribute[] infos, int originalIndex)
        {
            if (infos is null)
            {
                throw new ArgumentNullException(nameof(infos));
            }
            if (originalIndex >= 0 && originalIndex < infos.Length)
            {
                var id = infos[originalIndex].ID;
                int counter = -1;
                for (var idx = 0; idx < originalIndex + 1; idx++)
                {
                    if (id.Equals(infos[idx].ID))
                        counter++;
                }
                return counter;
            }
            else if (originalIndex == 0 && infos.Length == 0)
            {
                return -1;
            }
            else
                throw new IndexOutOfRangeException($"index should between 0~{infos.Length - 1}");
        }

        public static (ushort VID, ushort PID) GetID(this UniversalSerialBus.Device device)
        {
            var attr = (UniversalSerialBus.UsbAttribute)device.GetAttribute(typeof(UniversalSerialBus.UsbAttribute));
            return attr.ID;
        }
    }
}