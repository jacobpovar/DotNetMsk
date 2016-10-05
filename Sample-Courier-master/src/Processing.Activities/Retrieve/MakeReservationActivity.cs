namespace Processing.Activities.Retrieve
{
    using System;
    using System.Threading.Tasks;

    using MassTransit;
    using MassTransit.Courier;


    public class MakeReservationActivity : Activity<MakeReservationArguments, ReservationLog>
    {
        public Task<ExecutionResult> Execute(ExecuteContext<MakeReservationArguments> context)
        {
            var guestsCount = context.Arguments.GuestsCount;

            Serilog.Log.Information("Making reservation for {0} guests", guestsCount);

            //// типа делаем бронирование.
            var reservationId = NewId.NextGuid();

            Serilog.Log.Information("Created reservation: {0}", reservationId);

            return Task.FromResult(
                context.Completed<ReservationLog>(new Log(context.Arguments.RequestId, reservationId)));
        }
        
        public async Task<CompensationResult> Compensate(CompensateContext<ReservationLog> context)
        {
            Serilog.Log.Information("Cancelling reservation {0}", context.Log.ReservationId);
            
            await Task.Delay(0);
            return context.Compensated();
        }

        private class Log : ReservationLog
        {
            public Log(Guid requestId, Guid reservationId)
            {
                this.RequestId = requestId;
                this.ReservationId = reservationId;
            }

            public Guid RequestId { get; }

            public Guid ReservationId { get; }
        }
    }
}