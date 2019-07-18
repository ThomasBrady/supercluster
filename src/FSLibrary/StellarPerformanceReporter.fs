// Copyright 2019 Stellar Development Foundation and contributors. Licensed
// under the Apache License, Version 2.0. See the COPYING file at the root
// of this distribution or at http://www.apache.org/licenses/LICENSE-2.0

module StellarPerformanceReporter

open StellarCoreHTTP
open StellarCorePeer
open StellarDestination
open StellarNetworkCfg
open System

type Timer =
    {
        mean : decimal
        min : decimal
        max : decimal
        stdDev : decimal
        median : decimal
        per75th : decimal
        per95th : decimal
        per99th : decimal
    }

    static member FromGenericTimer (t: Metrics.GenericTimer) =
        {
            mean = decimal(t.Mean)
            min = decimal(t.Min)
            max = decimal(t.Max)
            stdDev = decimal(t.Stddev)
            median = decimal(t.Median)
            per75th = decimal(t.``75``)
            per95th = decimal(t.``95``)
            per99th = decimal(t.``99``)
        }

type PerformanceRow =
    {
        time : DateTime
        txtype : string
        accounts : int
        expectedTxs : int
        appliedTxs : int
        txRate : int
        batchSize : int
        txsPerLedgeMean : decimal
        txsPerLedgeStdDev : decimal
        loadStepRate : Option<double>
        loadStepStdDev : Option<double>
        nominate : Timer
        prepare : Timer
        close : Timer
        meanRate : decimal
    }

    member self.ToCsvRow =
        let valueOrNan x =
            match x with
            | None -> Double.NaN
            | Some v -> float(v)
        PerformanceCsv.Row(
            self.time.ToString(),
            self.txtype,
            self.accounts,
            self.expectedTxs,
            self.appliedTxs,
            self.txRate,
            self.batchSize,
            self.txsPerLedgeMean,
            self.txsPerLedgeStdDev,
            valueOrNan self.loadStepRate,
            valueOrNan self.loadStepStdDev,
            self.nominate.mean,
            self.nominate.min,
            self.nominate.max,
            self.nominate.stdDev,
            self.nominate.median,
            self.nominate.per75th,
            self.nominate.per95th,
            self.nominate.per99th,
            self.prepare.mean,
            self.prepare.min,
            self.prepare.max,
            self.prepare.stdDev,
            self.prepare.median,
            self.prepare.per75th,
            self.prepare.per95th,
            self.prepare.per99th,
            self.close.mean,
            self.close.min,
            self.close.max,
            self.close.stdDev,
            self.close.median,
            self.close.per75th,
            self.close.per95th,
            self.close.per99th,
            self.meanRate
        )

type PerformanceReporter(networkCfg: NetworkCfg) =
    let networkCfg = networkCfg
    let mutable data: Map<string, List<PerformanceRow>> = Map.empty

    member self.GetPerformanceMetrics (p: Peer) txtype accounts expectedTxs txRate batchSize =
        let metrics = p.GetMetrics
        {
            time = DateTime.Now
            txtype = txtype
            accounts = accounts
            expectedTxs = expectedTxs
            appliedTxs = metrics.LedgerTransactionApply.Count
            txRate = txRate
            batchSize = batchSize
            txsPerLedgeMean = decimal(metrics.LedgerTransactionCount.Mean)
            txsPerLedgeStdDev = decimal(metrics.LedgerTransactionCount.Stddev)
            loadStepRate = Option.map (fun (x: Metrics.GenericTimer) -> x.MeanRate) metrics.LoadgenStepSubmit
            loadStepStdDev = Option.map (fun (x: Metrics.GenericTimer) -> x.Stddev) metrics.LoadgenStepSubmit
            nominate = Timer.FromGenericTimer(metrics.ScpTimingNominated)
            prepare = Timer.FromGenericTimer(metrics.ScpTimingExternalized)
            close = Timer.FromGenericTimer(metrics.LedgerLedgerClose)
            meanRate = decimal(metrics.LedgerLedgerClose.MeanRate)
        }

    member self.RecordPerformanceMetrics txtype accounts expectedTxs txRate batchSize f =
        networkCfg.EachPeer (fun p-> p.ClearMetrics())
        f()
        networkCfg.EachPeer (fun p->
            let metrics = self.GetPerformanceMetrics p txtype accounts expectedTxs txRate batchSize
            if not (data.ContainsKey(p.ShortName))
            then
                data <- data.Add(p.ShortName, [])
            else
                true |> ignore
            let newPeerData = List.append data.[p.ShortName] [metrics]
            data <- Map.add p.ShortName newPeerData data
        )

    member self.DumpPerformanceMetrics (destination: Destination) =
        let dumpPeerPerformanceMetrics (destination: Destination) ns (p: Peer) =
            let toCsvRow (x: PerformanceRow) =
                x.ToCsvRow
            let name = p.ShortName
            if data.ContainsKey name
            then
                let csv = new PerformanceCsv(data.[name] |> (Seq.map toCsvRow) |> Seq.toList)
                destination.WriteString ns (sprintf "%s.perf" name) (csv.SaveToString('\t'))

        networkCfg.EachPeer (dumpPeerPerformanceMetrics destination networkCfg.NamespaceProperty)
