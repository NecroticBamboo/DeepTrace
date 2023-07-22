﻿// This file was auto-generated by ML.NET Model Builder. 
using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
namespace DeepTrace
{
    public partial class MLModel1
    {
        /// <summary>
        /// model input class for MLModel1.
        /// </summary>
        #region model input class
        public class ModelInput
        {
            [ColumnName(@"Q1min")]
            public string Q1min { get; set; }

            [ColumnName(@"Q1max")]
            public string Q1max { get; set; }

            [ColumnName(@"Q1avg")]
            public string Q1avg { get; set; }

            [ColumnName(@"Q1mean")]
            public string Q1mean { get; set; }

            [ColumnName(@"Q2min")]
            public string Q2min { get; set; }

            [ColumnName(@"Q2max")]
            public string Q2max { get; set; }

            [ColumnName(@"Q2avg")]
            public string Q2avg { get; set; }

            [ColumnName(@"Q2mean")]
            public string Q2mean { get; set; }

            [ColumnName(@"Q3min")]
            public string Q3min { get; set; }

            [ColumnName(@"Q3max")]
            public string Q3max { get; set; }

            [ColumnName(@"Q3avg")]
            public string Q3avg { get; set; }

            [ColumnName(@"Q3mean")]
            public string Q3mean { get; set; }

            [ColumnName(@"Q4min")]
            public string Q4min { get; set; }

            [ColumnName(@"Q4max")]
            public string Q4max { get; set; }

            [ColumnName(@"Q4avg")]
            public string Q4avg { get; set; }

            [ColumnName(@"Q4mean")]
            public string Q4mean { get; set; }

            [ColumnName(@"Q5min")]
            public string Q5min { get; set; }

            [ColumnName(@"Q5max")]
            public string Q5max { get; set; }

            [ColumnName(@"Q5avg")]
            public string Q5avg { get; set; }

            [ColumnName(@"Q5mean")]
            public string Q5mean { get; set; }

            [ColumnName(@"Name")]
            public string Name { get; set; }

        }

        #endregion

        /// <summary>
        /// model output class for MLModel1.
        /// </summary>
        #region model output class
        public class ModelOutput
        {
            [ColumnName(@"Q1min")]
            public string Q1min { get; set; }

            [ColumnName(@"Q1max")]
            public float[] Q1max { get; set; }

            [ColumnName(@"Q1avg")]
            public float[] Q1avg { get; set; }

            [ColumnName(@"Q1mean")]
            public float[] Q1mean { get; set; }

            [ColumnName(@"Q2min")]
            public float[] Q2min { get; set; }

            [ColumnName(@"Q2max")]
            public float[] Q2max { get; set; }

            [ColumnName(@"Q2avg")]
            public float[] Q2avg { get; set; }

            [ColumnName(@"Q2mean")]
            public float[] Q2mean { get; set; }

            [ColumnName(@"Q3min")]
            public float[] Q3min { get; set; }

            [ColumnName(@"Q3max")]
            public float[] Q3max { get; set; }

            [ColumnName(@"Q3avg")]
            public float[] Q3avg { get; set; }

            [ColumnName(@"Q3mean")]
            public float[] Q3mean { get; set; }

            [ColumnName(@"Q4min")]
            public string Q4min { get; set; }

            [ColumnName(@"Q4max")]
            public float[] Q4max { get; set; }

            [ColumnName(@"Q4avg")]
            public float[] Q4avg { get; set; }

            [ColumnName(@"Q4mean")]
            public float[] Q4mean { get; set; }

            [ColumnName(@"Q5min")]
            public float[] Q5min { get; set; }

            [ColumnName(@"Q5max")]
            public float[] Q5max { get; set; }

            [ColumnName(@"Q5avg")]
            public float[] Q5avg { get; set; }

            [ColumnName(@"Q5mean")]
            public float[] Q5mean { get; set; }

            [ColumnName(@"Name")]
            public uint Name { get; set; }

            [ColumnName(@"Features")]
            public float[] Features { get; set; }

            [ColumnName(@"PredictedLabel")]
            public string PredictedLabel { get; set; }

            [ColumnName(@"Score")]
            public float[] Score { get; set; }

        }

        #endregion

        private static string MLNetModelPath = Path.GetFullPath("MLModel1.zip");

        public static readonly Lazy<PredictionEngine<ModelInput, ModelOutput>> PredictEngine = new Lazy<PredictionEngine<ModelInput, ModelOutput>>(() => CreatePredictEngine(), true);

        /// <summary>
        /// Use this method to predict on <see cref="ModelInput"/>.
        /// </summary>
        /// <param name="input">model input.</param>
        /// <returns><seealso cref=" ModelOutput"/></returns>
        public static ModelOutput Predict(ModelInput input)
        {
            var predEngine = PredictEngine.Value;
            return predEngine.Predict(input);
        }

        private static PredictionEngine<ModelInput, ModelOutput> CreatePredictEngine()
        {
            var mlContext = new MLContext();
            ITransformer mlModel = mlContext.Model.Load(MLNetModelPath, out var _);
            return mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);
        }
    }
}