using System;

namespace AI
{
    public class NeuralNetwork
    {
        private static Random random;
        private static int INPUT_NODES;
        private static int HIDDEN_NODES;
        private static int OUTPUT_NODES;

        private double[] inputs;
        private double[][] inputHiddenWeights;
        private double[] hiddenBiases;
        private double[] hiddenOutputs;
        private double[][] hiddenOutputWeights;
        private double[] outputBiases;
        private double[] outputs;

        private int TotalWeights => (INPUT_NODES * HIDDEN_NODES) + (HIDDEN_NODES * OUTPUT_NODES) + HIDDEN_NODES + OUTPUT_NODES;


        public NeuralNetwork(int inputNodes, int hiddenNodes, int outputNodes)
        {
            random = new Random(0);
            INPUT_NODES = inputNodes;
            HIDDEN_NODES = hiddenNodes;
            OUTPUT_NODES = outputNodes;
            Initialize();
        }

        private void Initialize()
        {
            // Input, hidden and output layers (with biases and weights for hidden and output layer)
            inputs = new double[INPUT_NODES];
            inputHiddenWeights = CreateMatrix(INPUT_NODES, HIDDEN_NODES);
            hiddenBiases = new double[HIDDEN_NODES];
            hiddenOutputs = new double[HIDDEN_NODES];
            hiddenOutputWeights = CreateMatrix(HIDDEN_NODES, OUTPUT_NODES);
            outputBiases = new double[OUTPUT_NODES];
            outputs = new double[OUTPUT_NODES];

            // Initialize weights
            InitializeWeights();
        }

        #region Initialize
        private void InitializeWeights()
        {
            // Initialize weights and biases to small random values.
            double[] initialWeights = new double[TotalWeights];
            double lo = -0.01;
            double hi = 0.01;
            for (int i = 0; i < initialWeights.Length; ++i)
                initialWeights[i] = (hi - lo) * random.NextDouble() + lo;
            this.SetWeights(initialWeights);
        }
        private void SetWeights(double[] weights)
        {
            // Copy weights and biases in weights[] array to inputHiddenWeights, hiddenBiases, hiddenOutputWeights and outputBiases.
            int k = 0; // Points into weights parameter.
            for (int i = 0; i < INPUT_NODES; ++i)
                for (int j = 0; j < HIDDEN_NODES; ++j)
                    inputHiddenWeights[i][j] = weights[k++];
            for (int i = 0; i < HIDDEN_NODES; ++i)
                hiddenBiases[i] = weights[k++];
            for (int i = 0; i < HIDDEN_NODES; ++i)
                for (int j = 0; j < OUTPUT_NODES; ++j)
                    hiddenOutputWeights[i][j] = weights[k++];
            for (int i = 0; i < OUTPUT_NODES; ++i)
                outputBiases[i] = weights[k++];
        }
        private double[][] CreateMatrix(int rows, int columns)
        {
            double[][] result = new double[rows][];
            for (int r = 0; r < result.Length; ++r)
                result[r] = new double[columns];
            return result;
        }
        #endregion Initialize

        // GetWeights necessary? 
        // This method is usually used after and/or before a NN has completed its training, to display the current weights. 
        public double[] GetWeights()
        {
            // Returns the current set of weights, presumably after training.
            double[] result = new double[TotalWeights];
            int k = 0; // Points to each weight in a weight array to be taken from.
            for (int i = 0; i < inputHiddenWeights.Length; ++i)
                for (int j = 0; j < inputHiddenWeights[0].Length; ++j)
                    result[k++] = inputHiddenWeights[i][j];
            for (int i = 0; i < hiddenBiases.Length; ++i)
                result[k++] = hiddenBiases[i];
            for (int i = 0; i < hiddenOutputWeights.Length; ++i)
                for (int j = 0; j < hiddenOutputWeights[0].Length; ++j)
                    result[k++] = hiddenOutputWeights[i][j];
            for (int i = 0; i < outputBiases.Length; ++i)
                result[k++] = outputBiases[i];
            return result;
        }
        public static void ShowWeights(double[] vector, int valsPerRow, int decimals, bool newLine)
        {
            for (int i = 0; i < vector.Length; ++i)
            {
                if (i % valsPerRow == 0) Console.WriteLine("");
                Console.Write(vector[i].ToString("F" + decimals).PadLeft(decimals + 4) + " ");
            }
            if (newLine == true) Console.WriteLine("");
        }
    }
}
