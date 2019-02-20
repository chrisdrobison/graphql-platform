using System;
using System.Collections.Generic;
using System.Linq;

namespace HotChocolate.Stitching.Merge.Handlers
{
    public abstract class TypeMergeHanlderBase<T>
        : ITypeMergeHanlder
        where T : ITypeInfo
    {
        private readonly MergeTypeDelegate _next;

        protected TypeMergeHanlderBase(MergeTypeDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public void Merge(
            ISchemaMergeContext context,
            IReadOnlyList<ITypeInfo> types)
        {
            T left = types.OfType<T>().FirstOrDefault();

            if (left == null)
            {
                _next.Invoke(context, types);
            }
            else
            {
                var notMerged = types.OfType<T>().ToList();
                bool hasLeftovers = types.Count > notMerged.Count;

                while (notMerged.Count > 0)
                {
                    MergeNextType(context, notMerged);
                }

                if (hasLeftovers)
                {
                    _next.Invoke(context, types.NotOfType<T>());
                }
            }
        }

        private void MergeNextType(
            ISchemaMergeContext context,
            List<T> notMerged)
        {
            T left = notMerged[0];

            var readyToMerge = new List<T>();
            readyToMerge.Add(left);

            for (int i = 1; i < notMerged.Count; i++)
            {
                if (CanBeMerged(left, notMerged[i]))
                {
                    readyToMerge.Add(notMerged[i]);
                }
            }

            NameString newTypeName =
                TypeMergeHelpers.CreateName<T>(
                    context, readyToMerge);

            MergeTypes(context, readyToMerge, newTypeName);
            notMerged.RemoveAll(readyToMerge.Contains);
        }

        protected abstract bool CanBeMerged(T left, T right);

        protected abstract void MergeTypes(
            ISchemaMergeContext context,
            IReadOnlyList<T> types,
            NameString newTypeName);
    }
}
