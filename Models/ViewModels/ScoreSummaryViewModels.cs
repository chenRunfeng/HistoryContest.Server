﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HistoryContest.Server.Data;
using HistoryContest.Server.Models.Entities;

namespace HistoryContest.Server.Models.ViewModels
{
    public class ScoreBandCountViewModel
    {
        public int HigherThan90 { get; set; }
        public int HigherThan75 { get; set; }
        public int HigherThan60 { get; set; }
        public int Failed { get; set; }
        public int NotTested { get; set; }
    }

    public class ScoreSummaryByDepartmentViewModel
    {
        public Department DepartmentID { get; set; }
        public string CounselorName { get; set; }
        public int StudentCount { get; set; }
        public int MaxScore { get; set; }
        public double AverageScore { get; set; }
        public ScoreBandCountViewModel ScoreBandCount { get; set; }

        public void Update(Student student)
        { // 当有学生出成绩时，对院系概况进行更新
            if (student.Department != DepartmentID)
            {
                throw new ArgumentException("Student's department not consistent");
            }
            if (!student.IsTested)
            {
                throw new ArgumentException("Student hasn't been tested");
            }
            MaxScore = Math.Max((int)student.Score, MaxScore);
            AverageScore *= StudentCount - ScoreBandCount.NotTested;
            AverageScore += (double)student.Score;
            ScoreBandCount.NotTested -= 1;
            AverageScore /= StudentCount - ScoreBandCount.NotTested;
            switch ((int)student.Score / 10)
            {
                case 10:
                case 9:
                    ++ScoreBandCount.HigherThan90;
                    break;
                case 8:
                    ++ScoreBandCount.HigherThan75;
                    break;
                case 7:
                    if (student.Score >= 75)
                        ++ScoreBandCount.HigherThan75;
                    else
                        ++ScoreBandCount.HigherThan60;
                    break;
                case 6:
                    ++ScoreBandCount.HigherThan60;
                    break;
                default:
                    ++ScoreBandCount.Failed;
                    break;
            }
        }

        public static async Task<ScoreSummaryByDepartmentViewModel> GetAsync(UnitOfWork unitOfWork, Counselor counselor)
        { // 学生无改动时直接从缓存中取出，学生信息有改动时进行修改
            var summaryVMDictionary = unitOfWork.Cache.Dictionary<Department, ScoreSummaryByDepartmentViewModel>();
            var model = await summaryVMDictionary.GetAsync(counselor.Department);
            if (model == null)
            {
                model = new ScoreSummaryByDepartmentViewModel
                {
                    DepartmentID = counselor.Department,
                    CounselorName = counselor.Name,
                    StudentCount = await unitOfWork.StudentRepository.SizeByDepartment(counselor.Department),
                    MaxScore = await unitOfWork.StudentRepository.HighestScoreByDepartment(counselor.Department),
                    AverageScore = await unitOfWork.StudentRepository.AverageScoreByDepartment(counselor.Department),
                    ScoreBandCount =
                {
                    HigherThan90 = await unitOfWork.StudentRepository.ScoreHigherThanByDepartment(90, counselor.Department),
                    HigherThan75 = await unitOfWork.StudentRepository.ScoreHigherThanByDepartment(75, counselor.Department),
                    HigherThan60 = await unitOfWork.StudentRepository.ScoreHigherThanByDepartment(60, counselor.Department),
                    NotTested = await unitOfWork.StudentRepository.CountNotTestedByDepartment(counselor.Department)
                }
                };
                model.ScoreBandCount.Failed = model.StudentCount - model.ScoreBandCount.NotTested - model.ScoreBandCount.HigherThan60;
                await summaryVMDictionary.SetAsync(counselor.Department, model);
            }
            return model;
        }
    }

    public class ScoreSummaryOfSchoolViewModel
    {
        public int MaxScore { get; set; }
        public double AverageScore { get; set; }
        public ScoreBandCountViewModel ScoreBandCount { get; set; }
        public DateTime UpdateTime { get; set; }

        public static async Task<ScoreSummaryOfSchoolViewModel> GetAsync(UnitOfWork unitOfWork)
        { // 每隔10分钟自动过期，从而获得的model变为null，再重新创建一个
            var model = await unitOfWork.Cache.GetAsync<ScoreSummaryOfSchoolViewModel>("summary");
            if (model == null)
            {
                model = new ScoreSummaryOfSchoolViewModel
                {
                    MaxScore = await unitOfWork.StudentRepository.HighestScore(),
                    AverageScore = await unitOfWork.StudentRepository.AverageScore(),
                    ScoreBandCount =
                {
                    HigherThan90 = await unitOfWork.StudentRepository.ScoreHigherThan(90),
                    HigherThan75 = await unitOfWork.StudentRepository.ScoreHigherThan(75),
                    HigherThan60 = await unitOfWork.StudentRepository.ScoreHigherThan(60),
                    NotTested = await unitOfWork.StudentRepository.CountNotTested()
                },
                    UpdateTime = DateTime.Now
                };
                var size = await unitOfWork.StudentRepository.SizeAsync();
                model.ScoreBandCount.Failed = size - model.ScoreBandCount.NotTested - model.ScoreBandCount.HigherThan60;
                await unitOfWork.Cache.SetAsync("summary", model, TimeSpan.FromMinutes(10));
            }
            return model;
        }
    }
}
