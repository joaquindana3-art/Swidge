document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('checkoutForm');
    if (!form) return;

    const aceptar = document.getElementById('aceptaDevolucion');
    const boton = document.getElementById('confirmarCompra');
    const cardFields = document.querySelector('.checkout-card-fields');
    const paymentRadios = document.querySelectorAll('input[name="metodoPago"]');

    const numeroTarjeta = document.getElementById('numeroTarjeta');
    const cvvTarjeta = document.getElementById('cvvTarjeta');
    const vencimientoTarjeta = document.getElementById('vencimientoTarjeta');
    const titularTarjeta = document.getElementById('titularTarjeta');
    const direccionFacturacion = document.getElementById('direccionFacturacion');

    if (!aceptar || !boton || !cardFields || !numeroTarjeta || !cvvTarjeta || !vencimientoTarjeta || !titularTarjeta || !direccionFacturacion) {
        return;
    }

    function soloNumeros(valor) {
        return valor.replace(/\D/g, '');
    }

    function formatearTarjeta(valor) {
        const digits = soloNumeros(valor).slice(0, 16);
        return digits.replace(/(.{4})/g, '$1 ').trim();
    }

    function formatearCVV(valor) {
        return soloNumeros(valor).slice(0, 4);
    }

    function formatearVencimiento(valor) {
        const digits = soloNumeros(valor).slice(0, 4);
        if (digits.length <= 2) return digits;
        return digits.slice(0, 2) + '/' + digits.slice(2);
    }

    function actualizarBoton() {
        const enabled = aceptar.checked;
        boton.disabled = !enabled;
        boton.classList.toggle('is-disabled', !enabled);
    }

    function actualizarCamposPago() {
        const seleccionado = document.querySelector('input[name="metodoPago"]:checked');
        const tipoPago = seleccionado ? seleccionado.value : 'Tarjeta';
        const mostrarTarjeta = tipoPago === 'Tarjeta';
        cardFields.classList.toggle('visible', mostrarTarjeta);
    }

    numeroTarjeta.addEventListener('input', function () {
        this.value = formatearTarjeta(this.value);
    });

    cvvTarjeta.addEventListener('input', function () {
        this.value = formatearCVV(this.value);
    });

    vencimientoTarjeta.addEventListener('input', function () {
        this.value = formatearVencimiento(this.value);
    });

    titularTarjeta.addEventListener('input', function () {
        this.value = this.value.replace(/[^a-zA-ZÀ-ÿ\s]/g, '').replace(/\s{2,}/g, ' ').trimStart();
    });

    direccionFacturacion.addEventListener('input', function () {
        this.value = this.value.replace(/\s{2,}/g, ' ').trimStart();
    });

    paymentRadios.forEach(function (radio) {
        radio.addEventListener('change', function () {
            actualizarCamposPago();
        });
    });

    aceptar.addEventListener('change', actualizarBoton);
    actualizarBoton();
    actualizarCamposPago();

    form.addEventListener('submit', function (event) {
        if (!aceptar.checked) {
            event.preventDefault();
            aceptar.focus();
            alert('Debes aceptar los términos de devolución para confirmar la compra.');
            return;
        }

        const numero = numeroTarjeta.value.replace(/\s/g, '');
        const cvv = cvvTarjeta.value;
        const venc = vencimientoTarjeta.value;
        const titular = titularTarjeta.value.trim();
        const direccion = direccionFacturacion.value.trim();
        const hayDatosTarjeta = numero || cvv || venc || titular || direccion;

        if (document.querySelector('input[name="metodoPago"]:checked').value === 'Tarjeta' && hayDatosTarjeta) {
            if (!/^\d{16}$/.test(numero)) {
                event.preventDefault();
                alert('El número de tarjeta debe tener 16 dígitos.');
                numeroTarjeta.focus();
                return;
            }

            if (!/^\d{3,4}$/.test(cvv)) {
                event.preventDefault();
                alert('El CVV debe tener 3 o 4 dígitos.');
                cvvTarjeta.focus();
                return;
            }

            if (!/^(0[1-9]|1[0-2])\/\d{2}$/.test(venc)) {
                event.preventDefault();
                alert('La fecha de vencimiento debe estar en formato MM/AA.');
                vencimientoTarjeta.focus();
                return;
            }

            const [mes, anio] = venc.split('/').map(Number);
            const hoy = new Date();
            const fechaExp = new Date(2000 + anio, mes, 0, 23, 59, 59);

            if (fechaExp < new Date(hoy.getFullYear(), hoy.getMonth(), 1)) {
                event.preventDefault();
                alert('La tarjeta está vencida.');
                vencimientoTarjeta.focus();
                return;
            }

            if (titular.length < 3) {
                event.preventDefault();
                alert('Ingresá el nombre del titular.');
                titularTarjeta.focus();
                return;
            }

            if (direccion.length < 5) {
                event.preventDefault();
                alert('Ingresá una dirección de facturación válida.');
                direccionFacturacion.focus();
                return;
            }
        }

        event.preventDefault();
        const compraExitosaUrl = document.body.dataset.compraExitosaUrl || '/Home/CompraExitosa?id=' + (document.getElementById('entradaId')?.value || '0');
        window.location.href = compraExitosaUrl;
    });
});
