using System;
using System.Collections.Generic;
using System.Linq;

namespace LiveCoding.Linq.Grades
{
    class Program
    {
        public static Pupil[] ReadFromCsv()
        {
            string[][] matrix = MyFile.ReadStringMatrixFromCsv("grades.csv", false);
            return matrix.Select(line => new Pupil
            {
                Grades = line.Select(
                    (gradeText, idx) => new Tuple<int, int>(idx, GetGrade(gradeText)))
            }).ToArray();
        }

        private static int GetGrade(string gradeText)
        {
            if (string.IsNullOrEmpty(gradeText))
            {
                return 0;
            }

            return int.Parse(gradeText);
        }

        static void Main(string[] args)
        {
            Pupil[] pupils = ReadFromCsv();

            Console.WriteLine("-----------------------------------------------------");
            // Alle 1er zählen
            var cntOf1Er = pupils.Select(
                pupil =>
                    pupil.Grades.Count(grade => grade.Item2 == 1)
            ).Sum();
            Console.WriteLine($"Count of 1er: {cntOf1Er}");

            Console.WriteLine("-----------------------------------------------------");

            // Anzahl der Schüler, mit Notendurchschnitt <= 3; ohne Religion
            int cntOfGoodPupils =
                pupils.Select(
                    pupil => pupil.Grades
                        .Take(pupil.Grades.Count() - 1).Skip(1) // Filter für Religion
                            .Select(gradeTuple => gradeTuple.Item2).Where(grade => grade != 0)
                         .Average())
                    .Where(avgGrades => avgGrades <= 3).Count();

            Console.WriteLine($"Count of good Pupils: {cntOfGoodPupils}");

            Console.WriteLine("-----------------------------------------------------");

            // Anzahl der einzelnen Notenvorkommen auflisten
            var grpGrades =
                pupils.SelectMany(pupil =>
                        pupil.Grades
                            .Select(grade => grade.Item2)
                            .Where(grade => grade != 0))
                    .GroupBy(grade => grade).OrderBy(grade => grade.Key);

            foreach (var grade in grpGrades)
            {
                Console.WriteLine($"Grade '{grade.Key}' = {grade.Count()}");
            }

            Console.WriteLine("-----------------------------------------------------");

            // Notendurchschnitt pro Gegenstand ermitteln
            var subjectsWithAverage =
                    pupils.SelectMany(pupil => pupil.Grades
                    .Where(grade => grade.Item2 != 0)
                )
                .GroupBy(gradeTuple => gradeTuple.Item1)
                .Select(gradeGroup => new
                {
                    Subject = gradeGroup.Key,
                    Avg = gradeGroup
                        .Select(gradeGroup2 => gradeGroup2.Item2).Average()
                })
                .OrderBy(resultEntry => resultEntry.Subject);

            subjectsWithAverage
                .ToList()
                .ForEach(entry =>
                    Console.WriteLine($"Subject '{entry.Subject}' = {entry.Avg}"));

            Console.WriteLine("-----------------------------------------------------");


            IEnumerable<Tuple<int, string>> subjectMapping = new List<Tuple<int, string>>
            {
                new Tuple<int, string>(0, "Religion"),
                new Tuple<int, string>(1, "Deutsch"),
                new Tuple<int, string>(2, "Englisch"),
                new Tuple<int, string>(3, "Mathematik"),
                new Tuple<int, string>(4, "Wirtschaft und Recht"),
                new Tuple<int, string>(5, "Programmieren"),
                new Tuple<int, string>(6, "Datenbanken"),
                new Tuple<int, string>(7, "Netzwerksysteme"),
                new Tuple<int, string>(8, "Betriebswirtschaft"),
                new Tuple<int, string>(9, "Systemplanung"),
                new Tuple<int, string>(10, "Religion katholisch"),

            };

            var result = subjectsWithAverage.Join(
                subjectMapping,
                subjectsWithAverageItem => subjectsWithAverageItem.Subject,
                subjectMappingItem => subjectMappingItem.Item1,
                (calculatedAverage, mapping) => new
                {
                    Subject = mapping.Item2,
                    Avg = calculatedAverage.Avg
                })
                .OrderBy(entry => entry.Avg);

            result
                .ToList()
                .ForEach(entry =>
                    Console.WriteLine($"Subject '{entry.Subject}' = {entry.Avg}"));
        }
    }
}
