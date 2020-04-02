﻿using System;
using System.Collections.Concurrent;
using System.Threading;

namespace BulkWriter.Pipeline.Internal
{
    internal class TransformEtlPipelineStep<TOut> : EtlPipelineStep<TOut, TOut>
    {
        private readonly Action<TOut>[] _transformActions;

        public TransformEtlPipelineStep(EtlPipelineContext pipelineContext, BlockingCollection<TOut> inputCollection, params Action<TOut>[] transformActions) : base(pipelineContext, inputCollection)
        {
            _transformActions = transformActions;
        }

        protected override void RunCore(CancellationToken cancellationToken)
        {
            var enumerable = InputCollection.GetConsumingEnumerable(cancellationToken);

            foreach (var item in enumerable)
            {
                foreach (var transformAction in _transformActions)
                {
                    transformAction(item);
                }

                OutputCollection.Add(item, cancellationToken);
            }
        }
    }
}
