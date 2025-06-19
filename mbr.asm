org 0x7c00
bits 16

_start:
    jmp main

main:
    cli
    xor ax, ax
    mov ds, ax
    mov es, ax
    mov ss, ax
    mov sp, 0x7c00

    mov si, msg
    call print

hang:
    jmp hang

print:
    lodsb
    or al, al
    jz done
    mov ah, 0x0E
    int 0x10
    jmp print

done:
    ret

msg db "Nostalgia got you, and you cant do anything about it", 0
times 510 - ($-$$) db 0
dw 0xaa55