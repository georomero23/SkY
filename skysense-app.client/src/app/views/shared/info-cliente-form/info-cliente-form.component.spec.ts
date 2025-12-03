import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InfoClienteFormComponent } from './info-cliente-form.component';

describe('InfoClienteFormComponent', () => {
  let component: InfoClienteFormComponent;
  let fixture: ComponentFixture<InfoClienteFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InfoClienteFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InfoClienteFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
