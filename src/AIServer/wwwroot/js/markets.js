document.addEventListener('DOMContentLoaded', () => {
    const treeContainer = document.getElementById('market-tree');
    const chartsContainer = document.getElementById('charts');
    const noSelectionMessage = document.getElementById('no-selection');
    const chartCanvases = {
        day: document.getElementById('chart-day').getContext('2d'),
        hour: document.getElementById('chart-hour').getContext('2d'),
        minute: document.getElementById('chart-minute').getContext('2d'),
        second: document.getElementById('chart-second').getContext('2d')
    };
    let charts = {};

    // Fetch and render the initial market hierarchy
    fetch('/api/markets')
        .then(response => response.json())
        .then(data => {
            data.nodes.forEach(node => renderNode(node, treeContainer));
        })
        .catch(error => console.error('Error loading market tree:', error));

    // Render tree nodes recursively
    function renderNode(node, parentElement) {

        const li = document.createElement('li');
        const span = document.createElement('span');
        span.textContent = node.name;
        span.style.cursor = 'pointer';
        li.appendChild(span);
        li.appendChild(document.createElement('ul'));
        parentElement.appendChild(li);

        // Handle expandable nodes
        span.textContent = `[+] ${node.name}`;
        span.onclick = () => toggleNode(node, li, span);

        // Handle market (epic) selection
        if (node.markets?.length > 0) {
            const ul = document.createElement('ul');
            ul.style.display = 'none';

            node.markets.forEach(market => {
                const marketLi = document.createElement('li');
                marketLi.textContent = `${market.instrumentName} (${market.epic})`;
                marketLi.style.cursor = 'pointer';
                marketLi.onclick = () => loadCharts(market.epic);
                ul.appendChild(marketLi);
            });

            li.appendChild(ul);
        }
    }

    // Toggle node expansion
    function toggleNode(node, li, span) {

        const ul = li.querySelector('ul');

        if (ul.style.display != "block") {
            fetch(`/api/markets/${node.id}`)
                .then(response => response.json())
                .then(data => {

                    if (data.nodes && data.nodes.length > 0) {
                        data.nodes.forEach(node => renderNode(node, ul));
                    }

                    if (data.markets && data.markets.length > 0) {
                        renderMarkets(data.markets, ul);
                    }

                    ul.style.display = 'block';
                    span.textContent = `[-] ${node.name}`;
                })
                .catch(error => console.error('Error loading market tree:', error));
        }
        else {
            ul.style.display = 'none';
            span.textContent = `[+] ${node.name}`;
        }
    }

    function renderMarkets(markets, ul) {
        markets.forEach(market => {
            const li = document.createElement('li');
            const span = document.createElement('span');
            span.textContent = market.epic;
            span.style.cursor = 'pointer';
            span.classList.add("market");

            li.appendChild(span);
            span.onclick = () => {
                document.querySelectorAll('span.market')
                    .forEach(span => span.classList.remove('selected'));

                span.classList.add("selected");
                loadCharts(market.epic);
            };

            ul.appendChild(li);
        });
    }

    // Load and render charts for selected epic
    function loadCharts(epic) {
        fetch(`/api/pricing/${epic}`, {
            method: 'GET',
            headers: { 'Content-Type': 'application/json' }
        })
            .then(response => response.json())
            .then(data => {
                // Hide "no selection" message and show charts
                noSelectionMessage.style.display = 'none';
                chartsContainer.style.display = 'block';

                // Destroy existing charts if they exist
                Object.keys(charts).forEach(key => charts[key].destroy());

                // Render new charts
                charts.day = renderChart(chartCanvases.day, data.day, 'Day');
                charts.hour = renderChart(chartCanvases.hour, data.hour, 'Hour');
                charts.minute = renderChart(chartCanvases.minute, data.minute, 'Minute');
                charts.second = renderChart(chartCanvases.second, data.second, 'Second');
            })
            .catch(error => console.error('Error loading pricing data:', error));
    }

    // Render a single chart
    function renderChart(ctx, data, title) {
        const labels = data.map(d => d.snapshotTime);
        const prices = data.map(d => d.closePrice.bid);

        return new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: title,
                    data: prices,
                    borderColor: 'blue',
                    fill: false
                }]
            },
            options: {
                responsive: true,
                scales: {
                    x: { title: { display: true, text: 'Time' } },
                    y: { title: { display: true, text: 'Price' } }
                }
            }
        });
    }
});