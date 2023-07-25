﻿﻿// This file was auto-generated by ML.NET Model Builder. 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers.FastTree;
using Microsoft.ML.Trainers;
using Microsoft.ML;

namespace DeepTrace
{
    public partial class MLModel1
    {
        /// <summary>
        /// Retrains model using the pipeline generated as part of the training process. For more information on how to load data, see aka.ms/loaddata.
        /// </summary>
        /// <param name="mlContext"></param>
        /// <param name="trainData"></param>
        /// <returns></returns>
        public static ITransformer RetrainPipeline(MLContext mlContext, IDataView trainData)
        {
            var pipeline = BuildPipeline(mlContext);
            var model = pipeline.Fit(trainData);

            return model;
        }

        /// <summary>
        /// build the pipeline that is used from model builder. Use this function to retrain model.
        /// </summary>
        /// <param name="mlContext"></param>
        /// <returns></returns>
        public static IEstimator<ITransformer> BuildPipeline(MLContext mlContext)
        {
            // Data process configuration with pipeline data transformations
            var pipeline = mlContext.Transforms.Text.FeaturizeText(inputColumnName:@"Q1max",outputColumnName:@"Q1max")      
                                    .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName:@"Q1avg",outputColumnName:@"Q1avg"))      
                                    .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName:@"Q1mean",outputColumnName:@"Q1mean"))      
                                    .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName:@"Q2min",outputColumnName:@"Q2min"))      
                                    .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName:@"Q2max",outputColumnName:@"Q2max"))      
                                    .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName:@"Q2avg",outputColumnName:@"Q2avg"))      
                                    .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName:@"Q2mean",outputColumnName:@"Q2mean"))      
                                    .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName:@"Q3min",outputColumnName:@"Q3min"))      
                                    .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName:@"Q3max",outputColumnName:@"Q3max"))      
                                    .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName:@"Q3avg",outputColumnName:@"Q3avg"))      
                                    .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName:@"Q3mean",outputColumnName:@"Q3mean"))      
                                    .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName:@"Q4max",outputColumnName:@"Q4max"))      
                                    .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName:@"Q4avg",outputColumnName:@"Q4avg"))      
                                    .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName:@"Q4mean",outputColumnName:@"Q4mean"))      
                                    .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName:@"Q5min",outputColumnName:@"Q5min"))      
                                    .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName:@"Q5max",outputColumnName:@"Q5max"))      
                                    .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName:@"Q5avg",outputColumnName:@"Q5avg"))      
                                    .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName:@"Q5mean",outputColumnName:@"Q5mean"))      
                                    .Append(mlContext.Transforms.Concatenate(@"Features", new []{@"Q1max",@"Q1avg",@"Q1mean",@"Q2min",@"Q2max",@"Q2avg",@"Q2mean",@"Q3min",@"Q3max",@"Q3avg",@"Q3mean",@"Q4max",@"Q4avg",@"Q4mean",@"Q5min",@"Q5max",@"Q5avg",@"Q5mean"}))      
                                    .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName:@"Name",inputColumnName:@"Name"))      
                                    .Append(mlContext.MulticlassClassification.Trainers.OneVersusAll(binaryEstimator:mlContext.BinaryClassification.Trainers.FastTree(new FastTreeBinaryTrainer.Options(){NumberOfLeaves=33,MinimumExampleCountPerLeaf=14,NumberOfTrees=4,MaximumBinCountPerFeature=1022,FeatureFraction=0.99999999,LearningRate=0.757926844134433,LabelColumnName=@"Name",FeatureColumnName=@"Features"}),labelColumnName: @"Name"))      
                                    .Append(mlContext.Transforms.Conversion.MapKeyToValue(outputColumnName:@"PredictedLabel",inputColumnName:@"PredictedLabel"));

            return pipeline;
        }
    }
}
