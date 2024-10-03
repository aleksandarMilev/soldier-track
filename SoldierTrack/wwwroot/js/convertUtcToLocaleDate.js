function convertUtcToLocaleDate (element) {
    let utcDate = element.getAttribute('data-utc');
    let localDate = new Date(utcDate + 'Z');

    let options = { day: '2-digit', month: 'short', year: 'numeric' };
    let formattedDate = localDate.toLocaleDateString('en-GB', options).replace(/(\d{1,2})/, '$1 ').trim();

    element.textContent = formattedDate;
}

document.querySelectorAll('.utc-date').forEach(x => convertUtcToLocaleDate(x));