export const numbersFormat = (x, n = 2) => {
    console.log('number', x, isNaN(x));
    if (isNaN(x) || isNaN(n)) return false;
    const m = Math.pow(10, n);
    return Math.round(x * m) / m;
};
