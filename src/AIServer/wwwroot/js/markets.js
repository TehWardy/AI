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
        .then(data => renderTree(data, treeContainer))
        .catch(error => console.error('Error loading market tree:', error));

    // Render tree nodes recursively
    function renderTree(nodes, parentElement) {
        nodes.forEach(node => {
            const li = document.createElement('li');
            const span = document.createElement('span');
            span.textContent = node.name;
            span.style.cursor = 'pointer';
            li.appendChild(span);
            parentElement.appendChild(li);

            // Handle expandable nodes
            if (node.nodes?.length > 0 || node.markets?.length > 0) {
                span.textContent = `[+] ${node.name}`;
                span.onclick = () => toggleNode(node, li, span);
            }

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
        });
    }

    // Toggle node expansion
    function toggleNode(node, li, span) {
        const ul = li.querySelector('ul');
        if (ul && ul.children.length > 0) {
            // Already loaded, just toggle visibility
            ul.style.display = ul.style.display === 'none' ? 'block' : 'none';
            span.textContent = ul.style.display === 'none' ? `[+] ${node.name}` : `[-] ${node.name}`;
        } else if (node.id) {
            // Fetch child nodes
            fetch(`/api/marketnavigation/${node.id}`)
                .then(response => response.json())
                .then(childData => {
                    const newUl = document.createElement('ul');
                    renderTree([childData], newUl);
                    li.appendChild(newUl);
                    span.textContent = `[-] ${node.name}`;
                })
                .catch(error => console.error('Error loading node:', error));
        }
    }

    // Load and render charts for selected epic
    function loadCharts(epic) {
        fetch('/api/pricing', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(epic)
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