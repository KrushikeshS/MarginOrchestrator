using System;
using System.Collections.Generic;
using System.Linq;
using Orchestrator.Domain;

namespace Orchestrator.Infrastructure;

public class CsvDisputeParser : IDisputeParser
{
    public IEnumerable<MarginBreak> ParseDisputeCsv(string csvText)
    {
        var list = new List<MarginBreak>();
        
        // Split string by carriage returns to process line by line
        var lines = csvText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        // Skip(1) ignores the CSV title header columns
        foreach (var line in lines.Skip(1))
        {
            var columns = line.Split(',');
            if (columns.Length < 4) continue; // Skip malformed lines

            // Map columns to properties
            var breakEvent = new MarginBreak(
                DisputeId: columns[0].Trim(),
                BrokerName: columns[1].Trim(),
                BalanceDifference: decimal.Parse(columns[2].Trim()),
                ExecutionDate: DateTime.Parse(columns[3].Trim())
            );

            list.Add(breakEvent);
        }

        return list;
    }
}
