namespace axGB.CPU
{
    public class InteruptHandler
    {
        private Processor processor
        {
            get; init;
        }

        public InteruptHandler(Processor processor)
        {
            this.processor = processor;
        }

        public bool IME { get; set; }
        public bool IsInterruptRequested
        {
            get
            {
                return processor.memory.IE != 0;
            }
        }

        /// <summary>
        /// Process pending interupts.
        /// </summary>
        public void ProcessInterupts()
        {
            // I have no idea yet
            switch (processor.memory.IF)
            {
                default:
                    break;
            }
        }
    }
}