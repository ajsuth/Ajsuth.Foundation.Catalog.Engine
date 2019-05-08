// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CategoriesComponent.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ajsuth.Foundation.Catalog.Engine.Components
{
    using Sitecore.Commerce.Core;

    /// <inheritdoc />
    /// <summary>
    /// The category relationships component.
    /// </summary>
    public class CategoryHierarchyComponent : Component
    {
        /// <summary>
        /// The parent entity list
        /// </summary>
        public string ParentEntityList { get; set; }
    }
}

