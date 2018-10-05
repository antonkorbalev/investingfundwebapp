function getProfitsToolTip(desc, profit, profitPercent) {
    return '<div class="graph-tooltip"><div><strong>' + desc + '</strong></div>' + '<div>Profit:&nbsp;<strong>' + profit.toLocaleString() + '</strong>&nbsp;RUB</div><div>(<strong>' + profitPercent.toLocaleString() + '</strong>&nbsp;%)</div></div>';
}