using Pharmacy.System.Core.Models;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Globalization;

namespace Pharmacy.System.Core.Extensions
{
    public static class InventoryItemExtensions
    {
        /// <summary>
        /// Converts inventory quantities from base units into a readable multi-level
        /// packaging representation.
        /// </summary>
        /// <remarks>
        /// Uses each inventory item's <see cref="SupplyItem.PackagingLevelsSnapshot"/>
        /// rather than the product's current packaging configuration. Levels are applied
        /// from largest to smallest. Quantities with the same unit name are combined across
        /// the supplied items. Any remainder is expressed in the specified base unit, or
        /// the product form's base unit when no override is supplied.
        /// </remarks>
        /// <param name="inventoryItems">Inventory items whose quantities should be formatted.</param>
        /// <param name="remaining">
        /// <see langword="true"/> formats <see cref="InventoryItem.OnHandBaseUnits"/>;
        /// <see langword="false"/> formats <see cref="InventoryItem.ReceivedBaseUnits"/>.
        /// </param>
        /// <param name="baseUnit">Optional name to use for the remaining base units.</param>
        /// <returns>A string such as <c>10 boxes + 5 strips + 20 tablets</c>; an empty string if no positive quantity is found.</returns>
        public static string CalculateQuantities(
            this IEnumerable<InventoryItem> inventoryItems,
            bool remaining = true,
            string baseUnit = null)
        {
            var quantities = new Dictionary<string, decimal>(
                StringComparer.OrdinalIgnoreCase);

            foreach (var item in inventoryItems)
            {
                var units = remaining ? item.OnHandBaseUnits : item.ReceivedBaseUnits;

                var packagingLevels = item.SupplyItem.PackagingLevelsSnapshot?
                    .Where(x =>
                        x.QuantityOfBaseUnit > 0 &&
                        !string.IsNullOrWhiteSpace(x.UnitName))
                    .OrderByDescending(x => x.QuantityOfBaseUnit)
                    .ToList() ?? new();

                foreach (var level in packagingLevels)
                {
                    var quantity = decimal.Floor(
                        units / level.QuantityOfBaseUnit);

                    if (quantity <= 0)
                        continue;

                    var unitName = level.UnitName.Trim();

                    if (!quantities.ContainsKey(unitName))
                        quantities[unitName] = 0;

                    quantities[unitName] += quantity;

                    units -= quantity * level.QuantityOfBaseUnit;
                }

                // Remaining base units
                var baseUnitName = baseUnit ??
                    item.Product.ProductForm.BaseUnit.Name;

                if (units > 0)
                {
                    if (string.IsNullOrWhiteSpace(baseUnitName))
                        baseUnitName = "Base units";

                    baseUnitName = baseUnitName.Trim();

                    if (!quantities.ContainsKey(baseUnitName))
                        quantities[baseUnitName] = 0;

                    quantities[baseUnitName] += units;
                }
            }

            return string.Join(
                " + ",
                quantities
                    .Where(x => x.Value > 0)
                    .Select(x =>
                        $"{x.Value.ToString("0.####", CultureInfo.InvariantCulture)} {x.Key}"));
        }

        /// <summary>
        /// Converts inventory item quantity from base units into a readable multi-level
        /// packaging representation.
        /// </summary>
        /// <remarks>
        /// Uses each inventory item's <see cref="SupplyItem.PackagingLevelsSnapshot"/>
        /// rather than the product's current packaging configuration. Levels are applied
        /// from largest to smallest. Quantities with the same unit name are combined across
        /// the supplied items. Any remainder is expressed in the specified base unit, or
        /// the product form's base unit when no override is supplied.
        /// </remarks>
        /// <param name="inventoryItem">Inventory item whose quantity should be formatted.</param>
        /// <param name="remaining">
        /// <see langword="true"/> formats <see cref="InventoryItem.OnHandBaseUnits"/>;
        /// <see langword="false"/> formats <see cref="InventoryItem.ReceivedBaseUnits"/>.
        /// </param>
        /// <param name="baseUnit">Optional name to use for the remaining base units.</param>
        /// <returns>A string such as <c>10 boxes + 5 strips + 20 tablets</c>; an empty string if no positive quantity is found.</returns>
        public static string CalculateQuantity(
            this InventoryItem inventoryItem,
            bool remaining = true,
            string baseUnit = null)
        {
            return new List<InventoryItem> { inventoryItem }
                .CalculateQuantities(remaining, baseUnit);
        }
    }
}
