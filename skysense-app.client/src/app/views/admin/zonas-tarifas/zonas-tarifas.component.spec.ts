import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ZonasTarifasComponent } from './zonas-tarifas.component';

describe('ZonasTarifasComponent', () => {
  let component: ZonasTarifasComponent;
  let fixture: ComponentFixture<ZonasTarifasComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ZonasTarifasComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ZonasTarifasComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
