﻿using System;
using System.Collections.Concurrent;
using System.Threading;

namespace BulkWriter.Pipeline.Internal
{
    internal class ProjectEtlPipelineStep<TIn, TOut> : EtlPipelineStep<TIn, TOut>
    {
        private readonly Func<TIn, TOut> _projectionFunc;

        public ProjectEtlPipelineStep(EtlPipelineContext pipelineContext, BlockingCollection<TIn> inputCollection, Func<TIn, TOut> projectionFunc) : base(pipelineContext, inputCollection)
        {
            _projectionFunc = projectionFunc ?? throw new ArgumentNullException(nameof(projectionFunc));
        }

        protected override void RunCore(CancellationToken cancellationToken)
        {
            var enumerable = InputCollection.GetConsumingEnumerable(cancellationToken);

            foreach (var item in enumerable)
            {
                var result = _projectionFunc(item);
                OutputCollection.Add(result, cancellationToken);
            }
        }
    }
}