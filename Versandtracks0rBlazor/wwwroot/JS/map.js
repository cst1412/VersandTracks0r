
var map;

function createLeafletMap(id) {
    map = L.map(id).setView([0, 0], 12);
    L.tileLayer('http://{s}.tile.osm.org/{z}/{x}/{y}.png').addTo(map);
}

function showPath(history) {
    console.log(history);
    const points = history.map(e => L.latLng(e.lat, e.long));
    const mapItems = [];
    map.fitBounds(L.latLngBounds(points));

    // checken was ist wenn 0 drin ist
    //const path = L.polyline(points, { className: 'tracking_line', snakingSpeed: 500 });

    const path = L.motion.polyline(points, {
        className: 'tracking_line',
    }, {
        easing: L.Motion.Ease.easeInOutQuart,
        auto: true,
        duration: 2000,
    }, {
        removeOnEnd: false,
        icon: L.divIcon({
            html: "<div class='material-icons trackingIcon'>🚚</div>",
            iconSize: L.point(0, 0)
        })
    }).addTo(map);

    map.addLayer(path);
    mapItems.push(path);

    var start = history[0]; // lila
    var current = history[history.length - 1]; // grün

    var greenIcon = new L.Icon({
        iconUrl: 'https://cdn.rawgit.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-green.png',
        shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/0.7.7/images/marker-shadow.png',
        iconSize: [25, 41],
        iconAnchor: [12, 41],
        popupAnchor: [1, -34],
        shadowSize: [41, 41]
    });

    var violetIcon = new L.Icon({
        iconUrl: 'https://cdn.rawgit.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-violet.png',
        shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/0.7.7/images/marker-shadow.png',
        iconSize: [25, 41],
        iconAnchor: [12, 41],
        popupAnchor: [1, -34],
        shadowSize: [41, 41]
    });

    points.forEach((point, i) => {
        const marker = L.marker(point, { autoPan: true });

        const entry = history[i];

        // ugly af monkaS
        var test = history.filter(e => e.location == entry.location && e != entry);

        if (test.every(t => t.updatedAt < entry.updatedAt)) {
            //entry.lat + " " + " " + entry.long + " " + 
            marker.bindTooltip(entry.location + " <br> " + formatDateTime(entry.updatedAt), {
                permanent: true,
            }).openTooltip();

            if (start == entry) {
                marker.setIcon(violetIcon);
            }

            if (current == entry) {
                marker.setIcon(greenIcon);
            }

            map.addLayer(marker);
            mapItems.push(marker);
        }
    });
}

function formatDateTime(date_string) {
    return new Date(date_string).toLocaleString();
}